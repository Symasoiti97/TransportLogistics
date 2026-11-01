---
name: tl-db-inspect
description: Read-only queries against TL's local Neo4j (Tariff and locations graph), OSM PostGIS (osm_locations) and the shared Postgres container (Identity, --db tl-identity). Use to check counts, data shape or a row. Triggers (RU): «посмотри в базе», «сколько локаций», «проверь граф», «запрос к Neo4j», «запрос к Postgres», «db inspect».
---

# tl-db-inspect

Read-only. Run `.claude/skills/tl-db-inspect/scripts/db_query.py` (it calls `docker exec` into local containers).

## Targets

| Target | Container | Database | Notes |
|---|---|---|---|
| `neo4j` | `tl-neo4j-1` | Neo4j (Cypher) | user `neo4j`; password `NEO4J_PASSWORD` or `password123` |
| `osm` | `tl-postgis-osm-1` | `osm_locations` | countries, regions, cities, ports, railways, terminals, warehouses |
| `identity` | `postgres` | `--db <name>` required | shared with other projects; list databases with `--db postgres` |

## Examples

```bash
python3 .claude/skills/tl-db-inspect/scripts/db_query.py neo4j "MATCH (l:Location) RETURN l.Type, count(*)"
python3 .claude/skills/tl-db-inspect/scripts/db_query.py osm "select count(*) from cities"
python3 .claude/skills/tl-db-inspect/scripts/db_query.py identity --db tl-identity "select table_name from information_schema.tables where table_schema='public'"
```

Containers must run: `docker compose --profile infra up -d postgis-osm neo4j`.

## Rules

- The script refuses writes. Cypher must start with a read clause and contain no write keyword. SQL must start with SELECT, WITH, SHOW, EXPLAIN, TABLE or VALUES and contain no DDL/DML keyword. Postgres sessions are READ ONLY.
- Do not write, migrate or run `dotnet ef database update` here. Identity migrations: `.claude/rules/identity-postgres.md`.
- Never query `identity` without `--db`. The shared container also holds `ocr-db`, `shop` and others; query them only if asked.
- Report the exact query and its rows. Compare Neo4j counts with `osm` to check a sync (see `osm-import`).
- A refused query is a signal. Do not rewrite it to get around the check.
