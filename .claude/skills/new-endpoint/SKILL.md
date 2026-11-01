---
name: new-endpoint
description: Add a TL Web API endpoint (controller action, request DTO, use case wiring, swagger docs) to Tariffs.WebApi or an Identity API. Use when the user asks for a new route, HTTP method, or API action. Triggers (RU): «новый эндпоинт», «новый метод API», «добавь ручку», «new endpoint».
---

# new-endpoint

Rules: `.claude/rules/use-cases.md`, `errors.md`, `observability.md`. Build use cases with `new-use-case`, errors with `new-error`.

## 1. Place

- Tariff: `src/TransportLogistics/Tariff/Infrastructure/Tariffs.WebApi/Controllers/Tariff/TariffController.cs`; DTOs in `Controllers/Tariff/Dto/`.
- Identity: `src/Socials/Identity/Startups/Identity.Api/Controllers/`; DTOs in `Models/`.

## 2. Action (copy `TariffController`)

```csharp
/// <summary>
/// <Summary>
/// </summary>
/// <param name="tariffId" example="7f2c0960-4b9d-45d1-b00d-88d5a6f7b7a4">Tariff identifier</param>
[HttpPut("{tariffId:guid}/<segment>")]
[ProducesResponseType(typeof(TariffView), StatusCodes.Status200OK)]
public async Task<IActionResult> <Action>(
    [FromRoute] Guid tariffId,
    [FromBody] <Name>Request request,
    [FromServices] IUseCaseHandler<<Name>Command> commandHandler,
    [FromServices] IUseCaseHandler<GetTariffQuery, TariffView> queryHandler,
    CancellationToken cancellationToken)
{
    await commandHandler
        .HandleAsync(new <Name>Command(tariffId, ...), cancellationToken)
        .ConfigureAwait(false);

    var tariffView = await queryHandler
        .HandleAsync(new GetTariffQuery(tariffId), cancellationToken)
        .ConfigureAwait(false);

    return Ok(tariffView);
}
```

- Commands: `[FromServices] IUseCaseHandler<TCommand>`. Read-back: `IUseCaseHandler<GetTariffQuery, TariffView>`.
- Create: `StatusCode(StatusCodes.Status201Created, view)`. Update: `Ok`.
- Current user: `[FromServices] IUserContext userContext`, then `userContext.GetProfileId()`.
- Map DTO → command in the action. No business rules there.
- `[ProducesResponseType]` for every status returned; `<response code="...">` for action-specific ones. Class level already has 400, 404, 500.

## 3. Request DTO

```csharp
using System.ComponentModel.DataAnnotations;

namespace <Namespace>.Dto;

/// <summary>
/// <Summary>
/// </summary>
public sealed class <Name>Request
{
    /// <summary>
    /// <Field>
    /// </summary>
    /// <example>...</example>
    [NotDefault]
    public Guid <Field> { get; set; }
}
```

Validation: DataAnnotations (`[NotDefault]`, `[Required]`, `[Range]`).

## 4. Errors

Domain errors become ProblemDetails automatically; do not catch them in the controller. Register the type per `new-error`.

## 5. Verify

1. `dotnet build` of the API project (`Tariffs.WebApi.csproj` or `Identity.Api.csproj`), 0 errors.
2. Smoke test if dependencies run: Tariff needs Neo4j (`docker compose up -d neo4j`). `curl` the route, quote status and body. Note that the Tariff read query currently fails with 500 on Neo4j 5 (implicit grouping in `GetTariffQueryHandler`), so a read-back returns 500 even when the write succeeded. If dependencies are down, say the smoke test is not done.
3. Swagger: `GET /swagger/<api>/swagger.json` (names: `tariff-api`, `identity-api`) should contain the path.

## Do not

- No business logic in the controller.
- Do not return aggregates or domain types; return views or DTOs.
- Do not commit unless asked; no `Co-Authored-By`.
