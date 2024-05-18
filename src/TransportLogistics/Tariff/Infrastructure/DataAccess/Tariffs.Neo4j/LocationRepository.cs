using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class LocationRepository : ILocationRepository
{
    private readonly TariffDbContext _tariffDbContext;

    public LocationRepository(TariffDbContext tariffDbContext)
    {
        _tariffDbContext = tariffDbContext;
    }

    public async Task EnsureThatLocationsExists(IReadOnlySet<Guid> locationIds, CancellationToken cancellationToken)
    {
        var results = await _tariffDbContext
            .ReadAsync(
                query => query
                    .Unwind("$locationIds", "locationId")
                    .WithParam("locationIds", locationIds)
                    .Match("(l:Location {Id: locationId})")
                    .Return(l => l.Count())
                    .ResultsAsync,
                cancellationToken)
            .ConfigureAwait(false);

        var existingLocationCount = results.Single();
        if (existingLocationCount != locationIds.Count)
        {
            // TODO: GITHUB_ISSUE_TL-10
            throw new InvalidOperationException("Locations not found");
        }
    }
}