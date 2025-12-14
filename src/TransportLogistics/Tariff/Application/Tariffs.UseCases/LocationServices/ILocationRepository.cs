namespace TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

public interface ILocationRepository
{
    Task EnsureThatLocationsExists(IReadOnlySet<Guid> locationIds, CancellationToken cancellationToken);
}
