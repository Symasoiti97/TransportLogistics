using Application.Abstracts;
using Application.Abstracts.Repositories;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами цены
/// </summary>
internal sealed class SaveTariffPriceCommandHandler : ICommandHandler<SaveTariffPriceCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SaveTariffPriceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(SaveTariffPriceCommand command, CancellationToken cancellationToken = default)
    {
        var tariffRepository = _unitOfWork.GetRepository<ITariffRepository>();
        var tariff = await tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);
        if (tariff == null)
            throw new Exception("Tariff not found");

        var price = new Price(command.Price, command.CurrencyCode);
        tariff.SetPrice(price);

        tariffRepository.Update(tariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}