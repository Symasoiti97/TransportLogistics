using Application.Abstracts;
using Tariffs.Application.LocationServices;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик для сохранения тарифа с параметрами маршрута
/// </summary>
internal class SaveTariffRouteCommandHandler : ICommandHandler<SaveTariffRouteCommand>
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
        var tariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken);
        if (tariff == null)
            throw new Exception("Tariff not found");

        var locationIds = command.Points.Select(p => p.LocationId);
        var locations = await _locationRepository.GetAsync(locationIds, cancellationToken);
        var points = command.Points.Select(p => new Point(locations[p.LocationId], p.PointType, p.Order)).ToArray();
        var route = new Route(points);

        tariff.SetRoute(route);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}