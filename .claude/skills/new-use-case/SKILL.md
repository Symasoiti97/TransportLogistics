---
name: new-use-case
description: Add a TL use case (command or query record, handler, DI registration, tests) in the right BC layer. Use when the user asks for a new command, query or use case in Tariff or Identity. Triggers (RU): «новый юзкейс», «добавь команду», «добавь запрос», «new use case».
---

# new-use-case

Rules: `.claude/rules/use-cases.md`, `.claude/rules/architecture.md`.

## 1. Place

| BC | Command / handler | DI |
|---|---|---|
| Tariff | `src/TransportLogistics/Tariff/Application/Tariffs.UseCases/<Area>/` | `src/TransportLogistics/Tariff/Infrastructure/Tariffs.DependencyInjection/ServiceExtensions.cs` (`AddTariffServices`) |
| Identity | `src/Socials/Identity/Application/Identity.UseCases/<Area>/` | `src/Socials/Identity/Infrastructure/Identity.DependencyInjection/ServiceCollectionExtensions.cs` |

- `<Area>`: existing folder (`TariffServices`, `UserServices`, `TokenServices`) or new `<Name>Services`.
- Never register in `Tariff/Application/Tariffs.Application.DependencyInjection`: not in `TL.sln`, stale.

## 2. Record

- Command: `public sealed record <Name>Command(...) : IUseCase;`
- Query or command with result: `public sealed record <Name>Query(...) : IUseCase<<TResult>>;`
- `IUseCase` is in `TL.SharedKernel.Business.Aggregates`.
- Read-side query handler lives in Infrastructure next to its store, e.g. `Tariff/Infrastructure/DataAccess/Tariffs.Neo4j/Queries/GetTariffQueryHandler.cs`. Only the query and its view go to Application.

## 3. Handler (command side)

```csharp
using TL.SharedKernel.Application.Commands;

namespace <Namespace of the folder>;

internal sealed class <Name>CommandHandler(<IRepository> repository) : IUseCaseHandler<<Name>Command>
{
    public async Task HandleAsync(<Name>Command command, CancellationToken cancellationToken)
    {
        // load → call aggregate → save; no business rules here
    }
}
```

- Primary constructor; depend only on Application interfaces.
- Business rules go in the aggregate. Domain errors: `throw new <DomainError>(...)`; generic: `throw new Conflict().WithDetails("...")`. See `new-error`.
- Match the BC style: Identity uses `.ConfigureAwait(false)`, Tariff does not.

## 4. DI

```csharp
services.AddTransient<IUseCaseHandler<<Name>Command>, <Name>CommandHandler>();
```

Add next to the others in the BC's `AddXxxServices` method.

## 5. Tests

- Behaviour in the aggregate: `[Fact]`/`[Theory]` + FluentAssertions in `src/TransportLogistics/Tariff/Tests/Tariffs.UnitTests` (Tariff) or `src/Socials/Identity/Startups/Tests` (Identity).
- No handler test with mocks unless the handler has its own logic.

## 6. Verify

From `src/`: `dotnet build TL.sln` (0 errors), `dotnet test TL.sln --filter "FullyQualifiedName~<Name>"`. Report the real output.

## Do not

- Do not commit unless asked; no `Co-Authored-By` (project `CLAUDE.md`).
