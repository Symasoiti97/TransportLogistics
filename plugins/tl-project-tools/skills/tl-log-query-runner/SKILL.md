---
name: tl-log-query-runner
description: Run TL log queries against local Seq by logical fields (TraceId, service, level, event type, HTTP). Use to find logs, errors or a trace in TL services, or when another TL skill needs log lines. Triggers (RU): «найди логи», «ошибки в identity», «трейс», «логи по TraceId», «что писал сервис».
---

# tl-log-query-runner

Read-only. Runs a logical query on local Seq and prints lines oldest first. This is the only component that knows the Seq backend. Field schema: `.claude/rules/observability.md`. Field mapping: `build_filter` in `scripts/seq_query.py`. Other TL skills use this runner for queries; do not copy Seq syntax into them.

## Run

```bash
python3 ${CLAUDE_PLUGIN_ROOT}/skills/tl-log-query-runner/scripts/seq_query.py [filters] [options]
```

- Seq must run on `http://localhost:5341` (`docker compose up -d seq`). Override: `TL_SEQ_URL`. API key, if any: `TL_SEQ_API_KEY`. Local Seq runs without a key.

## Logical filters

| Flag | Seq clause |
|---|---|
| `--trace-id` | `@TraceId = '…'` |
| `--span-id` | `@SpanId = '…'` |
| `--service` | `@Resource.service.name`: `tariff-api`, `identity-api`, `sync-locations-job` |
| `--level` | `@Level`: Verbose, Debug, Info, Warn, Error, Fatal, OK (any case) |
| `--message` | `Contains(@Message, …)` |
| `--exception` | `Contains(@Exception, …)` |
| `--event-type` | `EventType` (Identity `EventProcessWorker` logs) |
| `--http-method` | `Method` ("Request finished" log) |
| `--http-path` | `Contains(Path, …)` |
| `--http-status` | `StatusCode` ("Request finished" log) |
| `--where` | raw Seq filter, ANDed with the rest |

Window: `--since 30m|2h|1d`, or `--from`/`--to` (ISO-8601 UTC). `--count` (default 200) = newest events fetched. `--json` prints raw Seq JSON. `--width 0` disables line truncation.

## Examples

```bash
seq_query.py --trace-id e37606b06d097dc7ec3bf948b0e70189
seq_query.py --service sync-locations-job --level error --since 2h
seq_query.py --event-type UserRegisterViaEmailTokenCreated --since 1d
seq_query.py --http-method POST --http-status 500 --since 2h
```

## Output

- Line: `timestamp level service traceId message`. `| EXC:` = first line of the exception.
- Stderr header: Seq filter and event count. Count = `--count` means truncated: raise `--count` or narrow the window.
- Empty result is not proof of absence: check `--since`, then field names.

## Gotchas

- Verified filters only: the fields above. Nested OTel attributes (`http.response.status_code`, `url.path`) did not filter reliably; use `--where` only after testing on a known event.
- System properties take `@` (`@Level`, `@SpanKind`). User properties do not (`EventType`, `Path`, `Method`, `StatusCode`).
- Seq returns newest first; the script sorts ascending.
- `Environment` is not reliable: `deployment.environment` reads `Production` on local runs.
- Read-only: starts no infrastructure, changes no data.
