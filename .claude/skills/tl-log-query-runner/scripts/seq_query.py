#!/usr/bin/env python3
"""Run a logical log query against the local Seq and print a compact result.

This is the only file that knows the Seq backend. Callers pass logical fields
from the observability log schema (.claude/rules/observability.md); this
script maps them to Seq filter syntax. Stdlib only.

Examples:
  seq_query.py --trace-id 4bf92f3577b34da6a3ce929d0e0e4736
  seq_query.py --service identity-api --level error --since 1h
  seq_query.py --event-type UserRegisterViaEmailTokenCreated --since 1d
  seq_query.py --http-status 500 --http-method POST --since 2h
  seq_query.py --where "@Level = 'ERROR' and Contains(@Message, 'Redis')"
"""

import argparse
import datetime as dt
import json
import os
import re
import sys
import urllib.error
import urllib.parse
import urllib.request

SEQ_URL = os.environ.get("TL_SEQ_URL", "http://localhost:5341").rstrip("/")
SEQ_API_KEY = os.environ.get("TL_SEQ_API_KEY")  # optional; local Seq runs without auth

SERVICES = {"tariff-api", "identity-api", "sync-locations-job"}
LEVELS = {"VERBOSE", "DEBUG", "INFO", "WARN", "ERROR", "FATAL", "OK"}
# Seq stores OTel levels uppercase. Accept any case from the caller.
LEVEL_ALIASES = {"TRACE": "VERBOSE", "INFORMATION": "INFO", "WARNING": "WARN",
                 "CRITICAL": "FATAL"}
SINCE_RE = re.compile(r"^(\d+)([smhd])$")


def q(value: str) -> str:
    """Quote a string literal for a Seq filter."""
    return "'" + value.replace("'", "''") + "'"


def parse_since(text: str) -> dt.datetime:
    m = SINCE_RE.match(text.strip())
    if not m:
        raise SystemExit(f"--since: expected like 30m, 2h, 1d; got {text!r}")
    n, unit = int(m.group(1)), m.group(2)
    delta = {"s": dt.timedelta(seconds=n), "m": dt.timedelta(minutes=n),
             "h": dt.timedelta(hours=n), "d": dt.timedelta(days=n)}[unit]
    return dt.datetime.now(dt.timezone.utc) - delta


def build_filter(args: argparse.Namespace) -> str:
    """Map logical fields to Seq filter clauses. Physical names verified against Seq."""
    clauses = []
    if args.trace_id:
        clauses.append(f"@TraceId = {q(args.trace_id)}")
    if args.span_id:
        clauses.append(f"@SpanId = {q(args.span_id)}")
    if args.service:
        clauses.append(f"@Resource.service.name = {q(args.service)}")
    if args.level:
        level = args.level.upper()
        level = LEVEL_ALIASES.get(level, level)
        clauses.append(f"@Level = {q(level)}")
    if args.message:
        clauses.append(f"Contains(@Message, {q(args.message)})")
    if args.exception:
        clauses.append(f"Contains(@Exception, {q(args.exception)})")
    if args.event_type:
        # Domain event type is a user property on the EventProcessWorker logs.
        clauses.append(f"EventType = {q(args.event_type)}")
    if args.http_method:
        # Property of the "Request finished" log of ASP.NET instrumentation.
        clauses.append(f"Method = {q(args.http_method.upper())}")
    if args.http_path:
        clauses.append(f"Contains(Path, {q(args.http_path)})")
    if args.http_status:
        clauses.append(f"StatusCode = {int(args.http_status)}")
    if args.where:
        clauses.append(f"({args.where})")
    return " and ".join(clauses)


def fetch(filter_text: str, args: argparse.Namespace) -> list:
    params = {"filter": filter_text, "count": str(args.count), "render": "true"}
    if args.since:
        params["fromDateUtc"] = parse_since(args.since).strftime("%Y-%m-%dT%H:%M:%SZ")
    if args.from_utc:
        params["fromDateUtc"] = args.from_utc
    if args.to_utc:
        params["toDateUtc"] = args.to_utc
    url = f"{SEQ_URL}/api/events?{urllib.parse.urlencode(params)}"
    req = urllib.request.Request(url, headers={"Accept": "application/json"})
    if SEQ_API_KEY:
        req.add_header("X-Seq-ApiKey", SEQ_API_KEY)
    try:
        with urllib.request.urlopen(req, timeout=20) as resp:
            return json.load(resp)
    except urllib.error.URLError as e:
        raise SystemExit(f"Seq is not reachable at {SEQ_URL}: {e.reason}. "
                         f"Start it: docker compose up -d seq") from e


def service_of(event: dict) -> str:
    for r in event.get("Resource", []):
        if r.get("Name") == "service":
            return (r.get("Value") or {}).get("name", "?")
    return "?"


def render(event: dict, width: int) -> str:
    ts = event.get("Timestamp", "")[:23]
    level = event.get("Level", "?")
    trace = event.get("TraceId") or "-"
    msg = (event.get("RenderedMessage") or "").replace("\n", " ")
    exc = event.get("Exception")
    line = f"{ts} {level:<7} {service_of(event):<20} {trace} {msg}"
    if exc:
        line += f" | EXC: {exc.splitlines()[0]}"
    return line[:width] if width else line


def main() -> int:
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("--trace-id")
    p.add_argument("--span-id")
    p.add_argument("--service", help=f"one of: {', '.join(sorted(SERVICES))}")
    p.add_argument("--level", help="Verbose|Debug|Info|Warn|Error|Fatal|OK (any case)")
    p.add_argument("--message", help="substring of the rendered message")
    p.add_argument("--exception", help="substring of the exception text")
    p.add_argument("--event-type", help="domain event type, e.g. UserRegisterViaEmailTokenCreated")
    p.add_argument("--http-method")
    p.add_argument("--http-path", help="substring of the request path")
    p.add_argument("--http-status", type=int)
    p.add_argument("--where", help="raw Seq filter, ANDed with the logical fields")
    p.add_argument("--since", help="relative window: 30m, 2h, 1d")
    p.add_argument("--from", dest="from_utc", help="ISO-8601 UTC start")
    p.add_argument("--to", dest="to_utc", help="ISO-8601 UTC end")
    p.add_argument("--count", type=int, default=200, help="max events to fetch (newest first)")
    p.add_argument("--width", type=int, default=240, help="truncate each line (0 = no limit)")
    p.add_argument("--json", action="store_true", help="print raw Seq JSON")
    args = p.parse_args()

    if args.service and args.service not in SERVICES:
        p.error(f"--service must be one of {sorted(SERVICES)}")
    if args.level and args.level.upper() not in LEVELS | set(LEVEL_ALIASES):
        p.error("--level must be one of Verbose|Debug|Info|Warn|Error|Fatal|OK")

    filter_text = build_filter(args)
    if not filter_text:
        p.error("give at least one filter (e.g. --trace-id or --since with --level)")

    events = fetch(filter_text, args)
    if args.json:
        print(json.dumps(events, ensure_ascii=False, indent=2))
        return 0

    # Seq returns newest first; print oldest first so a trace reads top-down.
    events.sort(key=lambda e: e.get("Timestamp", ""))
    print(f"# filter: {filter_text}", file=sys.stderr)
    print(f"# events: {len(events)}" + (" (limit reached, raise --count)" if len(events) >= args.count else ""), file=sys.stderr)
    for e in events:
        print(render(e, args.width))
    return 0


if __name__ == "__main__":
    sys.exit(main())
