using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

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
        ArgumentNullException.ThrowIfNull(command);

        var tariff = await _tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        await EnsureThatLocationsExistsAsync(command, cancellationToken);

        tariff.SetRoute(command.Route);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }

    private async Task EnsureThatLocationsExistsAsync(SaveTariffRouteCommand command, CancellationToken cancellationToken)
    {
        var locationIds = command.Route.Points.Select(p => p.LocationId).ToHashSet();
        await _locationRepository.EnsureThatLocationsExists(locationIds, cancellationToken).ConfigureAwait(false);
    }
}