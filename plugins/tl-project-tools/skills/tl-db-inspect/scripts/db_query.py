#!/usr/bin/env python3
"""Read-only queries against local TL databases through their Docker containers.

Targets:
  neo4j     container tl-neo4j-1, Cypher via cypher-shell (user neo4j, password from NEO4J_PASSWORD or password123)
  osm       container tl-postgis-osm-1, database osm_locations (PostGIS, OSM locations)
  identity  container postgres (shared with other projects!), database given by --db

Read-only is enforced twice: the query text must start with a read keyword (Cypher: MATCH/OPTIONAL MATCH/
WITH/RETURN/CALL db./SHOW/EXPLAIN; SQL: SELECT/WITH/SHOW/EXPLAIN/TABLE), and it must not contain write keywords.
Postgres runs in a READ ONLY transaction. Cypher is rejected when any write keyword appears.

Usage:
  db_query.py neo4j "MATCH (n) RETURN labels(n), count(*)"
  db_query.py osm "select count(*) from cities"
  db_query.py identity --db tl-identity "select table_name from information_schema.tables where table_schema='public'"
"""

import argparse
import os
import re
import subprocess
import sys

NEO4J_CONTAINER = "tl-neo4j-1"
OSM_CONTAINER = "tl-postgis-osm-1"
SHARED_PG_CONTAINER = "postgres"

CYPHER_READ_START = re.compile(r"^\s*(MATCH|OPTIONAL\s+MATCH|WITH|RETURN|CALL\s+db\.|SHOW|EXPLAIN|PROFILE|UNWIND)\b", re.I)
CYPHER_WRITE = re.compile(r"\b(CREATE|MERGE|DELETE|DETACH|SET|REMOVE|DROP|LOAD\s+CSV|CALL\s+apoc\.(create|merge|refactor|do|periodic))\b", re.I)
SQL_READ_START = re.compile(r"^\s*(SELECT|WITH|SHOW|EXPLAIN|TABLE|VALUES)\b", re.I)
SQL_WRITE = re.compile(r"\b(INSERT|UPDATE|DELETE|DROP|ALTER|CREATE|TRUNCATE|GRANT|REVOKE|COPY|VACUUM|REINDEX|CLUSTER|COMMENT|MERGE|CALL|DO)\b", re.I)


def check_cypher(query: str) -> None:
    if not CYPHER_READ_START.match(query):
        raise SystemExit("refused: Cypher must start with a read clause (MATCH, WITH, RETURN, CALL db., SHOW, EXPLAIN)")
    hit = CYPHER_WRITE.search(query)
    if hit:
        raise SystemExit(f"refused: write keyword '{hit.group(0)}' in Cypher; this tool is read-only")


def check_sql(query: str) -> None:
    if not SQL_READ_START.match(query):
        raise SystemExit("refused: SQL must start with SELECT, WITH, SHOW, EXPLAIN, TABLE or VALUES")
    hit = SQL_WRITE.search(query)
    if hit:
        raise SystemExit(f"refused: keyword '{hit.group(0)}' in SQL; this tool is read-only")


def run(cmd: list) -> int:
    return subprocess.run(cmd).returncode


def cypher(query: str) -> int:
    check_cypher(query)
    password = os.environ.get("NEO4J_PASSWORD", "password123")
    return run([
        "docker", "exec", NEO4J_CONTAINER, "cypher-shell",
        "-u", "neo4j", "-p", password, "--format", "plain", "--fail-fast", query,
    ])


def sql(container: str, database: str, query: str) -> int:
    check_sql(query)
    # Read-only transaction on the session level; psql stops on the first error.
    script = "SET SESSION CHARACTERISTICS AS TRANSACTION READ ONLY;\n" + query.rstrip().rstrip(";") + ";\n"
    proc = subprocess.run(
        ["docker", "exec", "-i", container, "psql", "-U", "postgres", "-d", database,
         "-v", "ON_ERROR_STOP=1", "-X", "-P", "pager=off"],
        input=script, text=True, capture_output=True,
    )
    sys.stdout.write(proc.stdout)
    if proc.returncode != 0:
        sys.stderr.write(proc.stderr)
    return proc.returncode


def main() -> int:
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("target", choices=["neo4j", "osm", "identity"])
    p.add_argument("query")
    p.add_argument("--db", help="database name (required for identity)")
    args = p.parse_args()

    if args.target == "neo4j":
        return cypher(args.query)
    if args.target == "osm":
        return sql(OSM_CONTAINER, "osm_locations", args.query)
    if not args.db:
        p.error("--db is required for identity (the shared container hosts other projects' databases)")
    return sql(SHARED_PG_CONTAINER, args.db, args.query)


if __name__ == "__main__":
    sys.exit(main())
