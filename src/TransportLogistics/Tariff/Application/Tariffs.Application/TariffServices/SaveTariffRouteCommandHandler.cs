using Application.Abstracts;
using Application.Abstracts.Repositories;
using Tariffs.Application.LocationServices;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик для сохранения тарифа с параметрами маршрута
/// </summary>
internal sealed class SaveTariffRouteCommandHandler : ICommandHandler<SaveTariffRouteCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SaveTariffRouteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(SaveTariffRouteCommand command, CancellationToken cancellationToken)
    {
        var tariffRepository = _unitOfWork.GetRepository<ITariffRepository>();

        var tariff = await tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);
        if (tariff == null)
            throw new Exception("Tariff not found");

        var locations = await GetLocationsAsync(command, cancellationToken).ConfigureAwait(false);
        var route = BuildRoute(command, locations);

        tariff.SetRoute(route);

        tariffRepository.Update(tariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyDictionary<Guid, Location>> GetLocationsAsync(SaveTariffRouteCommand command, CancellationToken cancellationToken)
    {
        var locationRepository = _unitOfWork.GetRepository<ILocationRepository>();

        var locationIds = command.Points.Select(p => p.LocationId);
        var locations = await locationRepository.FindAsync(locationIds, cancellationToken).ConfigureAwait(false);

        return locations.ToDictionary(location => location.Id, location => location);
    }

    private static Route BuildRoute(SaveTariffRouteCommand command, IReadOnlyDictionary<Guid, Location> locations)
    {
        var points = command.Points.Select(p => new Point(locations[p.LocationId], p.Type, p.Order)).ToArray();
        return new Route(points);
    }
}