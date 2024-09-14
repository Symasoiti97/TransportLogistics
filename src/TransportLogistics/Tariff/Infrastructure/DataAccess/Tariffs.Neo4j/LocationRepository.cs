using TL.SharedKernel.Infrastructure.Neo4j;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class LocationRepository : ILocationRepository
{
    private readonly ICypherGraphClientFactory _graphClientFactory;

    public LocationRepository(ICypherGraphClientFactory graphClientFactory)
    {
        ArgumentNullException.ThrowIfNull(graphClientFactory);

        _graphClientFactory = graphClientFactory;
    }

    public async Task EnsureThatLocationsExists(IReadOnlySet<Guid> locationIds, CancellationToken cancellationToken)
    {
        var query = await _graphClientFactory.GetCypherFluentQueryAsync(cancellationToken).ConfigureAwait(false);

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