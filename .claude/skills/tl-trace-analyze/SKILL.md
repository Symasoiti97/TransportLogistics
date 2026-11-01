---
name: tl-trace-analyze
description: Analyze one TL request or job by TraceId across services: span tree, services, first error from Seq, then an explanation. Use when the user gives a TraceId or asks what happened in a request, job or event chain. Triggers (RU): «проанализируй трейс», «что случилось с трейсом», «найди по трейсу», «trace-analyze».
---

# tl-trace-analyze

Read the trace through `tl-log-query-runner`; do not query Seq directly.

## Steps

1. Get the TraceId. Without one, find it first with `tl-log-query-runner` (`--message`, `--exception`, or `--service … --level error`).
2. Summary:
   ```bash
   python3 .claude/skills/tl-trace-analyze/scripts/trace_summary.py <TraceId>
   ```
   Prints services, levels, span tree (kind, service, elapsed, log count), spans with a parent outside the trace, first ERROR/FATAL with its exception.
3. Details of a span: `python3 .claude/skills/tl-log-query-runner/scripts/seq_query.py --span-id <SpanId> --width 0`
4. Answer from the lines you read. Quote the line or exception behind each claim; name the service where the failure is raised.

## Reading a trace

- Span whose parent is outside the trace = incoming request (e.g. curl `traceparent`). Normal for the root.
- `Consumer` span = event from Redis; its parent is the span that wrote the event (chain crosses services via the stream entry, see `observability.md`).
- `SyncLocationsJob` = one root span per cycle; one cycle = one TraceId.
- `OK` on a span = completed without error. The first `ERROR` log line is the failure point; read the lines after it.

## Rules

- Name a cause only after reading the full exception and the line that raises it.
- Confirm a fix or state with a request or query, not from the trace. If you cannot, say so.
- Report: key timeline lines, failing service and line, next check.

## Limits

- Summary reads up to 1000 events in the default Seq window; if it hits the limit, say so.
- Seq has no metrics; `Elapsed` is per span of this trace only.
