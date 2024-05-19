using TL.SharedKernel.Application.Commands;

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
        var tariff = await _tariffRepository.GetAsync(equipmentCommand.TariffId, cancellationToken).ConfigureAwait(false);

        tariff.SetCargoEquipment(equipmentCommand.CargoEquipment);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}