using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами груза
/// </summary>
internal sealed class SaveTariffCargoCommandHandler(ITariffRepository tariffRepository)
    : IUseCaseHandler<SaveTariffCargoEquipmentCommand>
{
    public async Task HandleAsync(SaveTariffCargoEquipmentCommand equipmentCommand, CancellationToken cancellationToken)
    {
        var tariff = await tariffRepository.GetAsync(equipmentCommand.TariffId, cancellationToken)
            .ConfigureAwait(false);

        tariff.SetCargoEquipment(equipmentCommand.CargoEquipment);

        await tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}