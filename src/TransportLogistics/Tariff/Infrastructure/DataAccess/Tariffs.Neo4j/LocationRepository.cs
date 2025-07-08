using TL.SharedKernel.Infrastructure.Neo4j;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class LocationRepository(ICypherGraphClientFactory graphClientFactory) : ILocationRepository
{
    public async Task EnsureThatLocationsExists(IReadOnlySet<Guid> locationIds, CancellationToken cancellationToken)
    {
        var query = await graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

        var results = await query
            .Unwind("$locationIds", "locationId")
            .WithParam("locationIds", locationIds)
            .Match("(l:Location {Id: locationId})")
            .Return(l => l.Count())
            .ResultsAsync
            .ConfigureAwait(false);

        var existingLocationCount = results.Single();
        if (existingLocationCount != locationIds.Count)
        {
            // TODO: GITHUB_ISSUE_TL-10
            throw new InvalidOperationException("Locations not found");
        }
    }
}