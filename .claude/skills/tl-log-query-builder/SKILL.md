---
name: tl-log-query-builder
description: Turn a plain-language request about TL logs (what, which service, which window) into tl-log-query-runner flags and run it. Use when the user describes logs without Seq syntax or a TraceId. Triggers (RU): «найди ошибки за», «логи identity за», «что писал сервис», «поищи в логах», «запрос к логам».
---

# tl-log-query-builder

Map the request to logical flags, run `tl-log-query-runner`. Never write Seq syntax here.

| User says | Flag |
|---|---|
| TraceId, trace | `--trace-id` (then `tl-trace-analyze`) |
| identity / tariff / sync or locations job | `--service identity-api` / `tariff-api` / `sync-locations-job` |
| errors, failures, exceptions | `--level error` (+ `--exception` if a type is named) |
| warnings | `--level warn` |
| domain event name | `--event-type <EventType>` |
| text in the message | `--message "…"` |
| POST/GET, path, status | `--http-method`, `--http-path`, `--http-status` |
| last hour, yesterday, a date | `--since 1h` / `--since 1d` or `--from`/`--to` |

Local time → convert to UTC before `--from`/`--to`. State the window you used.

## Procedure

1. Pick flags. If the task needs a service and none is given, ask once; otherwise search all services.
2. `python3 .claude/skills/tl-log-query-runner/scripts/seq_query.py <flags>`
3. Empty result: widen one thing at a time (window, then level, then message). Report which change found lines.
4. Hit `--count`: say so; offer a narrower window or larger count. Do not pull thousands of lines into context.
5. Show the command and the relevant lines; offer a TraceId for `tl-trace-analyze`.

## Do not

- Use `--where` unless no flag fits; raw Seq syntax belongs to the runner.
- Invent services: only `tariff-api`, `identity-api`, `sync-locations-job`.
- Claim a cause from log lines alone.
