#!/usr/bin/env python3
"""Summarize one trace: span tree, services, first error. Reads through tl-log-query-runner.

Usage: trace_summary.py <TraceId> [--count N]
Spans are events that carry Elapsed (activity spans); other events are logs of a span.
"""

import argparse
import collections
import json
import os
import subprocess
import sys

RUNNER = os.path.join(os.path.dirname(__file__), "..", "..", "tl-log-query-runner", "scripts", "seq_query.py")
ERROR_LEVELS = {"ERROR", "FATAL"}


def fetch(trace_id: str, count: int) -> list:
    out = subprocess.run(
        [sys.executable, RUNNER, "--trace-id", trace_id, "--count", str(count), "--json"],
        capture_output=True, text=True,
    )
    if out.returncode != 0:
        raise SystemExit(out.stderr.strip() or "runner failed")
    return json.loads(out.stdout or "[]")


def service(e: dict) -> str:
    for r in e.get("Resource", []):
        if r.get("Name") == "service":
            return (r.get("Value") or {}).get("name", "?")
    return "?"


def main() -> int:
    p = argparse.ArgumentParser()
    p.add_argument("trace_id")
    p.add_argument("--count", type=int, default=1000)
    args = p.parse_args()

    events = fetch(args.trace_id, args.count)
    if not events:
        print(f"No events for TraceId {args.trace_id} in the default window of Seq.")
        return 0
    events.sort(key=lambda e: e.get("Timestamp", ""))

    spans = {e["SpanId"]: e for e in events if "Elapsed" in e and e.get("SpanId")}
    logs_by_span = collections.Counter(e.get("SpanId") for e in events if "Elapsed" not in e)
    children = collections.defaultdict(list)
    for sid, s in spans.items():
        children[s.get("ParentId")].append(sid)

    services = collections.Counter(service(e) for e in events)
    levels = collections.Counter(e.get("Level") for e in events)
    t0, t1 = events[0]["Timestamp"][:23], events[-1]["Timestamp"][:23]

    print(f"TraceId {args.trace_id}")
    print(f"events {len(events)}, spans {len(spans)}, window {t0} .. {t1}")
    print("services: " + ", ".join(f"{s} ({n})" for s, n in services.most_common()))
    print("levels: " + ", ".join(f"{k} {v}" for k, v in levels.most_common()))

    print("\nSpan tree (kind, service, elapsed, log lines):")

    def walk(parent, depth):
        for sid in sorted(children.get(parent, []), key=lambda x: spans[x].get("Start", "")):
            s = spans[sid]
            name = (s.get("RenderedMessage") or "?")[:70].replace("\n", " ")
            print(f"{'  ' * depth}- {s.get('SpanKind')} {name} [{service(s)}] {s.get('Elapsed', '')[:12]} logs={logs_by_span.get(sid, 0)}")
            walk(sid, depth + 1)

    walk(None, 0)
    # Spans whose parent is outside this trace (e.g. incoming traceparent from a caller).
    for sid, s in spans.items():
        if s.get("ParentId") and s.get("ParentId") not in spans:
            name = (s.get("RenderedMessage") or "?")[:70].replace("\n", " ")
            print(f"- {s.get('SpanKind')} {name} [{service(s)}] parent {s.get('ParentId')} not in trace")
            walk(sid, 1)

    errors = [e for e in events if e.get("Level") in ERROR_LEVELS]
    print(f"\nerrors: {len(errors)}")
    if errors:
        first = errors[0]
        print(f"first error: {first['Timestamp'][:23]} [{service(first)}] SpanId {first.get('SpanId')}")
        print(f"  message: {(first.get('RenderedMessage') or '')[:200]}")
        if first.get("Exception"):
            print(f"  exception: {first['Exception'].splitlines()[0][:300]}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
