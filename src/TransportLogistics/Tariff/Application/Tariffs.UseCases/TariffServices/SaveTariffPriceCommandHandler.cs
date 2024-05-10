using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами цены
/// </summary>
internal sealed class SaveTariffPriceCommandHandler : ICommandHandler<SaveTariffPriceCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public SaveTariffPriceCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(SaveTariffPriceCommand command, CancellationToken cancellationToken = default)
    {
        var tariff = await _tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        tariff.SetPrice(command.Price);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}