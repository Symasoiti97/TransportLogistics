using Neo4jClient.Cypher;
using TL.SharedKernel.Application.Repositories;
using TL.SharedKernel.Infrastructure.Compress.Extensions;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class TariffRepository : IWriteRepository, ITariffRepository
{
    private readonly ILocationRepository _locationRepository;
    private readonly TariffDbContext _tariffDbContext;

    public TariffRepository(TariffDbContext tariffDbContext, ILocationRepository locationRepository)
    {
        _tariffDbContext = tariffDbContext;
        _locationRepository = locationRepository;
    }

    public async Task<Tariff?> FindAsync(Guid tariffId, CancellationToken cancellationToken)
    {
        var tariffResults = await _tariffDbContext.ReadAsync(
            query => query
                .Match("(tariff:Tariff {Id: $tariffId})")
                .OptionalMatch("(tariff)-[:HAS_ROUTE]->(route:Route)-[point:HAS_POINT]->(location:Location)")
                .WithParam("tariffId", tariffId)
                .Return(
                    (tariff, route, point, location) =>
                        new TariffResult
                        {
                            Tariff = tariff.As<TariffNode>(),
                            Route = route.As<RouteNode>(),
                            LocationPoints = Return.As<TariffResult.LocationPoint[]>(
                                "collect({Point: point, Location: location})")
                        })
                .ResultsAsync,
            cancellationToken).ConfigureAwait(false);

        var tariffResult = tariffResults.SingleOrDefault();

        return tariffResult is not null
            ? await MapToTariffAsync(tariffResult, cancellationToken).ConfigureAwait(false)
            : null;
    }

    public void Add(Tariff tariff)
    {
        _tariffDbContext.AddCommand(
            query =>
            {
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
                            CargoType = tariff.CargoType,
                            ContainerOwn = tariff.ContainerOwn,
                            ContainerSize = tariff.ContainerSize,
                            CreatedUtc = DateTime.UtcNow,
                            UpdatedUtc = DateTime.UtcNow,
                            IsDraft = tariff.IsDraft,
                            ManagerProfileId = tariff.ManagerProfileId
                        });

                if (tariff.Route is not null)
                {
                    query = query.Create("(t)-[:HAS_ROUTE]->(r)");
                }

                return query.ExecuteWithoutResultsAsync();
            });
    }

    public void Update(Tariff tariff)
    {
        _tariffDbContext.AddCommand(
            query =>
            {
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
                            CargoType = tariff.CargoType,
                            ContainerOwn = tariff.ContainerOwn,
                            ContainerSize = tariff.ContainerSize,
                            UpdatedUtc = DateTime.UtcNow,
                            IsDraft = tariff.IsDraft,
                            ManagerProfileId = tariff.ManagerProfileId
                        });

                query = query.Delete("oldHR");

                if (tariff.Route is not null)
                {
                    query = query.Create("(t)-[:HAS_ROUTE]->(r)");
                }

                return query.ExecuteWithoutResultsAsync();
            });
    }

    public void Delete(Tariff tariff)
    {
        _tariffDbContext.AddCommand(
            query => query
                .Match("(t:Tariff {Id: $tariffId})")
                .Set("t.DeletedUtc = $deletedUtc")
                .WithParams(new {tariffId = tariff.Id, deletedUtc = DateTime.UtcNow})
                .ExecuteWithoutResultsAsync());
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _tariffDbContext.SaveChangesAsync(cancellationToken);
    }

    private static ICypherFluentQuery BuildMergeRouteQuery(Route route, ICypherFluentQuery query)
    {
        foreach (var point in route.Points)
        {
            query = query
                .Match($"(l{point.Order}:Location {{Id: $locationId{point.Order}}})")
                .WithParam($"locationId{point.Order}", point.Location.Id);
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

    private async Task<Tariff> MapToTariffAsync(TariffResult result, CancellationToken cancellationToken)
    {
        var route = default(Route);
        if (result.Route is not null)
        {
            var locations = await _locationRepository
                .FindAsync(result.LocationPoints.Select(point => point.Location.Id), cancellationToken)
                .ConfigureAwait(false);
            var locationDictionary = locations.ToDictionary(location => location.Id, location => location);

            var points = result.LocationPoints
                .Select(
                    locationPoint =>
                        new Point(
                            locationDictionary[locationPoint.Location.Id],
                            locationPoint.Point.Type,
                            (ushort) locationPoint.Point.Order))
                .ToArray();

            route = new Route(points);
        }

        var price = default(Price);
        if (result.Tariff.Price.HasValue && result.Tariff.CurrencyCode is not null)
        {
            price = new Price(result.Tariff.Price.Value, result.Tariff.CurrencyCode);
        }

        return new Tariff(
            result.Tariff.Id,
            result.Tariff.ManagerProfileId,
            route,
            result.Tariff.ContainerOwn,
            result.Tariff.ContainerSize,
            result.Tariff.CargoType,
            price,
            result.Tariff.IsDraft);
    }

    private class TariffResult
    {
        public TariffNode Tariff { get; set; } = null!;
        public RouteNode? Route { get; set; }
        public LocationPoint[] LocationPoints { get; set; } = null!;

        internal class LocationPoint
        {
            public PointRelationship Point { get; set; } = null!;
            public LocationNode Location { get; set; } = null!;
        }
    }
}