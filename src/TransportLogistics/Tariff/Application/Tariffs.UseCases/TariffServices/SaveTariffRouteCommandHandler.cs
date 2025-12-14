using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

internal sealed class SaveTariffRouteCommandHandler(
    ITariffRepository tariffRepository,
    ILocationRepository locationRepository)
    : IUseCaseHandler<SaveTariffRouteCommand>
{
    public async Task HandleAsync(SaveTariffRouteCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        var tariff = await tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        await EnsureThatLocationsExistsAsync(command, cancellationToken);

        tariff.SetRoute(command.Route);

        await tariffRepository.UpdateAsync(tariff, cancellationToken);
    }

    private async Task EnsureThatLocationsExistsAsync(
        SaveTariffRouteCommand command,
        CancellationToken cancellationToken)
    {
        var locationIds = command.Route.Points.Select(p => p.LocationId).ToHashSet();
        await locationRepository.EnsureThatLocationsExists(locationIds, cancellationToken).ConfigureAwait(false);
    }
}
