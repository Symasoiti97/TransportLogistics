using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами груза
/// </summary>
internal sealed class SaveTariffCargoCommandHandler : ICommandHandler<SaveTariffCargoEquipmentCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public SaveTariffCargoCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(SaveTariffCargoEquipmentCommand equipmentCommand, CancellationToken cancellationToken = default)
    {
        var tariff = await _tariffRepository.FindAsync(equipmentCommand.TariffId, cancellationToken).ConfigureAwait(false);

        Error.Throw().TariffNotFoundIfNull(tariff, equipmentCommand.TariffId);

        tariff.SetCargoEquipment(equipmentCommand.ContainerSize, equipmentCommand.CargoType, equipmentCommand.ContainerOwn);

        _tariffRepository.Update(tariff);

        await _tariffRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}