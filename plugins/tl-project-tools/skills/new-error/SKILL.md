---
name: new-error
description: Add a TL domain error (subclass of NotFound, Conflict, InvalidValue or InvalidParameters) in the aggregate assembly and check the API exposes it in JSON and the <api>-errors swagger document. Use when the user asks for a new error, failure type or error response in Tariff or Identity. Triggers (RU): «новая ошибка», «добавь тип ошибки», «new error».
---

# new-error

Rules: `.claude/rules/errors.md`.

## 1. Create

- Place: `<Aggregate>/Errors/<Name>.cs`. Tariff: `src/TransportLogistics/Tariff/Domain/Tariff.Aggregates/AggregateTariff/Errors/`. Identity: `src/Socials/Identity/Businees/Identity.Aggregates/`.
- Base: `NotFound`, `Conflict`, `InvalidValue` or `InvalidParameters` (`TL.SharedKernel.Business.Aggregates`). Copy `TariffNotFound`:

```csharp
using TL.SharedKernel.Business.Aggregates;

namespace <Aggregate namespace>.Errors;

public class <Name> : NotFound
{
    public Guid <Id> { get; }

    /// <inheritdoc />
    public override string Message => "<Human-readable message>.";

    public <Name>(Guid <id>)
    {
        <Id> = <id>;
    }
}
```

- Throw: `throw new <Name>(id);` (implicit conversion to `ErrorException`). Generic case: `throw new Conflict().WithDetails("...")`.

## 2. Make the API scan it

The API reads only the assemblies in its `Program.cs` (`errorTypes`), used for JSON polymorphism and the `<api>-errors` swagger document.

- Tariff (`src/TransportLogistics/Tariff/Infrastructure/Tariffs.WebApi/Program.cs`): already includes `Tariff.Aggregates`. A new type there is picked up.
- Identity (`src/Socials/Identity/Startups/Identity.Api/Program.cs`): scans only SharedKernel (`typeof(AssemblyReference)` resolves to `TL.SharedKernel.Business.Aggregates`). **Identity's aggregate assembly is missing.** Latent: Identity has no errors yet. Before the first Identity error, add `typeof(TL.Socials.Identity.Business.Aggregates.AssemblyReference).Assembly` to `errorTypes` and tell the user.
- Never move an error to another assembly to dodge this.

## 3. Check swagger

- `<api>-errors` document must contain the schema. Swagger UI is only in Development.
- Needs running dependencies: Tariff → Neo4j (`docker compose --profile infra up -d neo4j`); Identity → Postgres, Redis. If not running, say the check is not done.
- Claim presence only after seeing it in the response.

## 4. Verify

From `src/`: `dotnet build TL.sln` (0 errors). Unit test in the aggregate test project only if the error has logic.

## Do not

- No exceptions for control flow when an aggregate can return a result (`rules/errors.md`).
- Do not commit unless asked.
