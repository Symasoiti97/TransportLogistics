using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

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
        var tariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        Error.Throw().TariffNotFoundIfNull(tariff, command.TariffId);

        tariff.SetPrice(command.Price);

        _tariffRepository.Update(tariff);

        await _tariffRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}