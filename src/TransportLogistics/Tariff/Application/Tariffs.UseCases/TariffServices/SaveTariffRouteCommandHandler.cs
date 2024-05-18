using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик для сохранения тарифа с параметрами маршрута
/// </summary>
internal sealed class SaveTariffRouteCommandHandler : ICommandHandler<SaveTariffRouteCommand>
{
    private readonly ITariffRepository _tariffRepository;
    private readonly ILocationRepository _locationRepository;

    public SaveTariffRouteCommandHandler(ITariffRepository tariffRepository, ILocationRepository locationRepository)
    {
        _tariffRepository = tariffRepository;
        _locationRepository = locationRepository;
    }

    public async Task HandleAsync(SaveTariffRouteCommand command, CancellationToken cancellationToken)
    {
        var tariff = await _tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        var locations = await GetLocationsAsync(command, cancellationToken).ConfigureAwait(false);
        var route = BuildRoute(command, locations);

        tariff.SetRoute(route);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }

    private async Task<IReadOnlyDictionary<Guid, Location>> GetLocationsAsync(
        SaveTariffRouteCommand command,
        CancellationToken cancellationToken)
    {
        var locationIds = command.Points.Select(p => p.LocationId);
        var locations = await _locationRepository.FindAsync(locationIds, cancellationToken).ConfigureAwait(false);

        return locations.ToDictionary(location => location.Id, location => location);
    }

    private static Route BuildRoute(SaveTariffRouteCommand command, IReadOnlyDictionary<Guid, Location> locations)
    {
        var points = command.Points.Select(p => new Point(locations[p.LocationId], p.Type, p.Order)).ToArray();
        return new Route(points);
    }
}