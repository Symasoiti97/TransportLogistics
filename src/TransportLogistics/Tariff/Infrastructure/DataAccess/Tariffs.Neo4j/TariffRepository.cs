using Neo4jClient.Cypher;
using TL.SharedKernel.Infrastructure.Compress.Extensions;
using TL.SharedKernel.Infrastructure.Neo4j;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class TariffRepository(ICypherGraphClientFactory graphClientFactory) : ITariffRepository
{
    public async Task<Tariff> GetAsync(Guid tariffId, CancellationToken cancellationToken)
    {
        var query = await graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

        var tariffResults = await query
            .Match("(tariff:Tariff {Id: $tariffId})")
            .OptionalMatch("(tariff)-[:HAS_ROUTE]->(route:Route)-[point:HAS_POINT]->(location:Location)")
            .WithParam("tariffId", tariffId)
            .Return((tariff, route, point, location) =>
                new TariffResult
                {
                    Tariff = tariff.As<TariffNode>(),
                    Route = route.As<RouteNode>(),
                    LocationPoints = Return.As<TariffResult.LocationPoint[]>(
                        "collect({Point: point, Location: location})")
                })
            .ResultsAsync.ConfigureAwait(false);

        var tariffResult = tariffResults.Single();
        if (tariffResult is null)
        {
            throw new TariffNotFound(tariffId);
        }

        return MapToTariff(tariffResult);
    }

    public async Task AddAsync(Tariff tariff, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tariff);

        var query = await graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

        if (tariff.Route is not null)
        {
            query = BuildMergeRouteQuery(tariff.Route, query);
        }

        query = query
            .Create("(t:Tariff $tariff)")
            .WithParam(
                "tariff",
                new TariffNode
                {
                    Id = tariff.Id,
                    Price = tariff.Price?.Value,
                    CurrencyCode = tariff.Price?.CurrencyCode,
                    CargoType = tariff.CargoEquipment?.CargoType,
                    ContainerOwn = tariff.CargoEquipment?.ContainerOwn,
                    ContainerSize = tariff.CargoEquipment?.ContainerSize,
                    CreatedUtc = DateTime.UtcNow,
                    UpdatedUtc = DateTime.UtcNow,
                    IsDraft = tariff.IsDraft,
                    ManagerProfileId = tariff.ManagerProfileId
                });

        if (tariff.Route is not null)
        {
            query = query.Create("(t)-[:HAS_ROUTE]->(r)");
        }

        await query.ExecuteWithoutResultsAsync();
    }

    public async Task UpdateAsync(Tariff tariff, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tariff);

        var query = await graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

        if (tariff.Route is not null)
        {
            query = BuildMergeRouteQuery(tariff.Route, query)
                .With("r");
        }

        query = query
            .Match("(t:Tariff {Id: $tariff.Id})")
            .OptionalMatch("(t)-[oldHR:HAS_ROUTE]->(:Route)")
            .Set(
                @"
                    t.Price = $tariff.Price,
                    t.CurrencyCode = $tariff.CurrencyCode,
                    t.CargoType = $tariff.CargoType,
                    t.ContainerOwn = $tariff.ContainerOwn,
                    t.ContainerSize = $tariff.ContainerSize,
                    t.UpdatedUtc = $tariff.UpdatedUtc,
                    t.IsDraft = $tariff.IsDraft,
                    t.ManagerProfileId = $tariff.ManagerProfileId")
            .WithParam(
                "tariff",
                new TariffNode
                {
                    Id = tariff.Id,
                    Price = tariff.Price?.Value,
                    CurrencyCode = tariff.Price?.CurrencyCode,
                    CargoType = tariff.CargoEquipment?.CargoType,
                    ContainerOwn = tariff.CargoEquipment?.ContainerOwn,
                    ContainerSize = tariff.CargoEquipment?.ContainerSize,
                    UpdatedUtc = DateTime.UtcNow,
                    IsDraft = tariff.IsDraft,
                    ManagerProfileId = tariff.ManagerProfileId
                });

        query = query.Delete("oldHR");

        if (tariff.Route is not null)
        {
            query = query.Create("(t)-[:HAS_ROUTE]->(r)");
        }

        await query.ExecuteWithoutResultsAsync();
    }

    private static ICypherFluentQuery BuildMergeRouteQuery(Route route, ICypherFluentQuery query)
    {
        foreach (var point in route.Points)
        {
            query = query
                .Match($"(l{point.Order}:Location {{Id: $locationId{point.Order}}})")
                .WithParam($"locationId{point.Order}", point.LocationId);
        }

        query = query
            .Merge("(r:Route {Hash: $route.Hash})")
            .OnCreate()
            .Set("r.Type = $route.Type")
            .WithParam(
                "route",
                new RouteNode
                {
                    Hash = route.Hash.Compress(),
                    Type = route.Type
                });

        foreach (var point in route.Points)
        {
            query = query
                .Merge($"(r)-[p{point.Order}:HAS_POINT]-(l{point.Order})")
                .OnCreate()
                .Set(
                    @$"p{point.Order}.Hash = $point{point.Order}.Hash, 
                                p{point.Order}.Type = $point{point.Order}.Type, 
                                p{point.Order}.Order = $point{point.Order}.Order")
                .WithParam(
                    $"point{point.Order}",
                    new PointRelationship
                    {
                        Hash = point.Hash.Compress(),
                        Type = point.Type,
                        Order = point.Order
                    });
        }

        return query;
    }

    private static Tariff MapToTariff(TariffResult result)
    {
        Route? route = null;
        if (result.Route is not null)
        {
            var points = result.LocationPoints
                .Select(locationPoint =>
                    new Point(
                        locationPoint.Location.Id,
                        locationPoint.Point.Type,
                        (ushort) locationPoint.Point.Order))
                .ToHashSet();

            route = new Route(points);
        }

        Price? price = null;
        if (result.Tariff is {Price: not null, CurrencyCode: not null})
        {
            price = new Price(result.Tariff.Price.Value, result.Tariff.CurrencyCode.Value);
        }

        CargoEquipment? cargoEquipment = null;
        if (result.Tariff is {CargoType: not null, ContainerOwn: not null, ContainerSize: not null})
        {
            cargoEquipment = new CargoEquipment(
                result.Tariff.CargoType.Value,
                result.Tariff.ContainerOwn.Value,
                result.Tariff.ContainerSize.Value);
        }

        return new Tariff(
            result.Tariff.Id,
            result.Tariff.ManagerProfileId,
            route,
            cargoEquipment,
            price,
            result.Tariff.IsDraft);
    }

    private sealed class TariffResult
    {
        public required TariffNode Tariff { get; init; }
        public RouteNode? Route { get; init; }
        public required LocationPoint[] LocationPoints { get; init; }

        internal sealed class LocationPoint
        {
            public required PointRelationship Point { get; init; }
            public required LocationNode Location { get; init; }
        }
    }
}