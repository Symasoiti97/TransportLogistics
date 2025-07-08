using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами цены
/// </summary>
internal sealed class SaveTariffPriceCommandHandler(ITariffRepository tariffRepository)
    : IUseCaseHandler<SaveTariffPriceCommand>
{
    public async Task HandleAsync(SaveTariffPriceCommand command, CancellationToken cancellationToken)
    {
        var tariff = await tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        tariff.SetPrice(command.Price);

        await tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}