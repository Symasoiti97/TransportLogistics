---
paths:
  - "src/**/UseCases/**"
  - "src/**/*.UseCases/**"
---

# Use cases: commands, queries, handlers

- Use cases live in `*.UseCases/<Area>/` in the Application layer. Each command or query is a record implementing `IUseCase` (no result) or `IUseCase<TResult>`.
- Handlers implement `IUseCaseHandler<TUseCase>` or `IUseCaseHandler<TUseCase, TResult>`. The same interface serves commands and queries (no separate Command/Query interfaces).
- Handlers are `internal sealed class` with dependencies in the primary constructor, for example `CreateTariffCommandHandler(ITariffRepository tariffRepository)`. Business logic stays in aggregates; the handler loads, calls, and saves.
- Register handlers in the BC's `*.DependencyInjection` (`AddTariffServices`, `AddIdentityServices`), not in Startups.
- Use cases depend only on Application-layer interfaces (repositories, read queries). They never reference Infrastructure types.
- Errors are domain `Error` subclasses (see `errors.md`), not exceptions for control flow.
