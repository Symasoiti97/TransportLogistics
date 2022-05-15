using Application.Abstracts;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами груза
/// </summary>
internal class SaveTariffCargoCommandHandler : ICommandHandler<SaveTariffCargoCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public SaveTariffCargoCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(SaveTariffCargoCommand command, CancellationToken cancellationToken = default)
    {
        var tariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken);
        if (tariff == null)
            throw new Exception("Tariff not found");

        tariff.SetCargoType(command.CargoType);
        tariff.SetContainerOwn(command.ContainerOwn);
        tariff.SetContainerSize(command.ContainerSize);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}