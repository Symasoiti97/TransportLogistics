# Architecture: layers and dependency direction

- Layers: `Startups → Infrastructure → Application → Business → System`.
- `System` exists only in `SharedKernel` (shared code without business logic).
- Each BC layer references the same-named `SharedKernel` layer (Business → Business, Application → Application, Infrastructure → Infrastructure) and the layer below it in its own BC.
- Startups reference SharedKernel only through Infrastructure, never directly.
- BCs (`Tariff`, `Identity`, `Locations`) must not reference each other directly. Tariff gets location data through `LocationServices` in its UseCases.
- Each layer is its own csproj.

What lives where:
- **Business**: aggregates, entities, value objects, business services. Folder `Domain` (Tariff) or `Businees` (Identity, historical typo); csproj `*.Aggregates`.
- **Application**: repository interfaces (for Business models) and read-query interfaces (for DTOs); use cases (`*.UseCases`). Repository interfaces such as `ITariffRepository` live here, not in Infrastructure.
- **Infrastructure**: repository and query implementations, `DataAccess/*`, `*.DependencyInjection` (`AddTariffServices`, `AddIdentityServices`), WebApi and Services.
- **Startups**: runnable apps: Web API, background services, utilities, Business unit tests.

The actual layout differs in places (`Tariffs.WebApi` is under `Infrastructure`, Tariff tests are under `Tariff/Tests`). Put new code according to the scheme above.
