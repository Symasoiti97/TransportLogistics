# Observability: logs, traces, TraceId

- Logging is OpenTelemetry over `ILogger` (`AddTlObservability()` in `SharedKernel.AspNet`). Do not add Serilog. Web services also call `AddTlAspNetCoreInstrumentation()`.
- Every `ActivitySource` is named `TL.<Context>` (for example `TL.Identity.Events`, `TL.Locations.SyncLocationsJob`). The OTel pattern `TL.*` makes them traced automatically; any other name is not exported.
- A `TraceId` is taken from `Activity.Current`. HTTP and HttpClient propagate W3C `traceparent` automatically.
- Events through Redis streams must carry `traceparent` in the stream entry: `AddEventMessage` writes it from `Activity.Current`, and `EventProcessor` restores it as the parent of the `Process event` span. Do not drop this field when changing the envelope.
- A worker without an incoming request (`SyncLocationsJob`) opens its own root span; one sync cycle is one `TraceId`.

## Log schema (logical fields)

| Field | Source |
|---|---|
| `TraceId`, `SpanId` | `Activity.Current` |
| `Service` | `ApiOptions.Name` or worker name |
| `Environment`, `Version` | configuration, assembly version |
| `Level`, `Message`, `Timestamp` | `ILogger` |
| `ExceptionType`, `ExceptionStack` | `Exception` |
| `HttpMethod`, `HttpPath`, `HttpStatus` | ASP.NET instrumentation |
| `EventType` | domain event (Identity `EventProcessWorker`) |

`ErrorType` is intentionally not added; if needed later, use tag `error.type` on the request span.

## Local backend: Seq

- `docker compose up -d seq` (port 5341). Ingest: `/ingest/otlp/v1/{logs,traces}`.
- Filters: `@TraceId = '<id>'`; `@Resource.service.name = 'identity-api'` (names: `tariff-api`, `identity-api`, `sync-locations-job`).
- HTTP API: `GET http://localhost:5341/api/events?filter=<filter>&count=N&render=true`.
- Seq has no metrics endpoint.
