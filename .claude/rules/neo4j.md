---
paths:
  - "src/TransportLogistics/Tariff/Infrastructure/DataAccess/Tariffs.Neo4j/**"
  - "src/SharedKernel/**"
  - "src/Locations/**"
---

# Neo4j: queries and connections

- Storage: Neo4j for Tariff (`Tariffs.Neo4j`) and for locations (`SyncLocationsJob` writes there). Connection settings are read from the `Neo4j` settings class in each app (`Neo4JSettings`); the client is built by `CypherGraphClientFactory` in `SharedKernel.DependencyInjection`.
- Get a query with `ICypherGraphClientFactory.GetCypherFluentQueryAsync(cancellationToken)`; don't create clients by hand.
- Pass values as parameters (`WithParam("tariffId", tariffId)`), never by string interpolation in Cypher.
- Map results to DTOs (`TariffResult`, `RouteNode`, `TariffNode`) in `Entities/`, then to the aggregate. Repositories return aggregates; queries return DTOs.
- Missing data: use `SingleOrDefault` and check for null before mapping (see `errors.md` for the known `Single()` issue in `TariffRepository.GetAsync`).
- Local instance: `docker compose --profile infra up -d neo4j` (port 7687). Password is `password123` in `docker-compose.yml` and in both `appsettings.json` files (Tariffs.WebApi, SyncLocationsJob).
