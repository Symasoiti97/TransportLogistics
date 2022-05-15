using Application.Abstracts;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами цены
/// </summary>
internal class SaveTariffPriceCommandHandler : ICommandHandler<SaveTariffPriceCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public SaveTariffPriceCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(SaveTariffPriceCommand command, CancellationToken cancellationToken = default)
    {
        var tariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken);
        if (tariff == null)
            throw new Exception("Tariff not found");

        var price = new Price(command.Price, command.CurrencyCode);
        tariff.SetPrice(price);

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}