using EnsureThat;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Entities;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class LocationRepository : ILocationRepository
{
    private readonly TariffDbContext _tariffDbContext;

    public LocationRepository(TariffDbContext tariffDbContext)
    {
        _tariffDbContext = tariffDbContext;
    }

    public async Task<Location[]> FindAsync(IEnumerable<Guid> locationIds, CancellationToken cancellationToken)
    {
        var uniqueLocationIds = locationIds.Distinct().ToArray();

        var results = await _tariffDbContext
            .ReadAsync(
                query => query
                    .Unwind("$locationIds", "locationId")
                    .WithParam("locationIds", uniqueLocationIds)
                    .Match("(l:Location {Id: locationId})")
                    .OptionalMatch("(l)<-[:HAS_LOCATION*]-(h:Location)")
                    .With("l + collect(h) as locations")
                    .Return<LocationNode[]>("locations")
                    .ResultsAsync,
                cancellationToken)
            .ConfigureAwait(false);

        var locationNodesArray = results.ToArray();

        if (locationNodesArray.Length != uniqueLocationIds.Length)
        {
            // TODO: GITHUB_ISSUE_TL-10
            throw new InvalidOperationException("Locations not found");
        }

        var locations = locationNodesArray.Select(locationNodes => MapToLocation(locationNodes)).ToArray();

        return locations!;
    }

    private static Location? MapToLocation(LocationNode[] locationNodes, int index = 0)
    {
        EnsureArg.HasItems(locationNodes, nameof(locationNodes));
        EnsureArg.IsGte(index, 0, nameof(index));

        var location = default(Location?);
        if (index < locationNodes.Length)
        {
            var locationNode = locationNodes[index];
            location = new Location(
                locationNode.Id,
                MapToLocation(locationNodes, index + 1),
                locationNode.Type);
        }

        return location;
    }
}