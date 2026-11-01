---
name: new-aggregate
description: Add a TL aggregate root (with events, value objects and repository interface) in the Business layer of Tariff or Identity. Use when the user asks for a new aggregate, entity, value object or domain event. Triggers (RU): «новый агрегат», «новая сущность», «новый value object», «new aggregate».
---

# new-aggregate

Rules: `.claude/rules/architecture.md`, `errors.md`, `tests.md`.

## 1. Place

| BC | Folder | Csproj |
|---|---|---|
| Tariff | `src/TransportLogistics/Tariff/Domain/Tariff.Aggregates/<Name>Aggregate/` | `Tariff.Aggregates` |
| Identity | `src/Socials/Identity/Businees/Identity.Aggregates/<Name>Aggregate/` | `Identity.Aggregates` |

- Namespace: `TL.TransportLogistics.Tariffs.Business.Aggregates.<Name>Aggregate` (Tariff), `TL.Socials.Identity.Business.Aggregates.<Name>Aggregate` (Identity). Match neighbouring files.
- Inside: `Errors/` (see `new-error`), `Events/`.
- Repository interface goes to Application next to the use cases (`ITariffRepository`: `Tariff/Application/Tariffs.UseCases/TariffServices/`), never Business or Infrastructure.

## 2. Aggregate root

```csharp
using TL.SharedKernel.Business.Aggregates;

namespace <Namespace>;

public sealed class <Name> : AggregateRoot<Guid>
{
    public <Type> <Property> { get; private set; }

    public <Name>(Guid id, <Type> <property>) : base(id)
    {
        Set<Property>(<property>);
    }

    public static <Name> Create(Guid id, <Type> <property>)
    {
        var item = new <Name>(id, <property>);

        item.Raise(<Name>Created.Create(item.Id));

        return item;
    }

    public void Set<Property>(<Type> value)
    {
        ArgumentNullException.ThrowIfNull(value);

        <Property> = value;
    }
}
```

- `AggregateRoot<TKey>` needs `TKey : struct`; use `Guid`.
- Rule violation: throw a domain error (`new <Error>(...)` or `new Conflict().WithDetails("...")`) in the setter or method.
- Raise events with `Raise(...)`; the handler saves and events are read from `Events`.
- Setters are `private set`; change state through methods.

## 3. Event

```csharp
using TL.SharedKernel.Business.Aggregates;

namespace <Namespace>.Events;

/// <inheritdoc />
public sealed class <Name>Created(Guid eventId, Guid <name>Id) : Event(eventId)
{
    public Guid <Name>Id { get; } = <name>Id;

    public static <Name>Created Create(Guid <name>Id) => new(Guid.NewGuid(), <name>Id);
}
```

## 4. Value object

Immutable `sealed record`, validation in the constructor (see `Price.cs` in Tariff `AggregateTariff`).

## 5. Repository interface

```csharp
namespace <Application namespace>;

public interface I<Name>Repository
{
    Task AddAsync(<Name> item, CancellationToken cancellationToken);
}
```

Only the methods use cases need. Implementation goes to Infrastructure `DataAccess/...`.

## 6. Tests

Aggregate test project: `src/TransportLogistics/Tariff/Tests/Tariffs.UnitTests` (Tariff) or `src/Socials/Identity/Startups/Tests` (Identity). Cover: create raises the event; each rule throws the expected error; valid setter value accepted. xunit + FluentAssertions.

## 7. Verify

From `src/`: `dotnet build TL.sln` (0 errors), `dotnet test TL.sln --filter "FullyQualifiedName~<Name>"`. Report real output.

## Do not

- No EF, Neo4j or Redis types in the aggregate.
- No repository implementation here.
- Do not commit unless asked; no `Co-Authored-By`.
