using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff.Errors;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
internal sealed class PublishTariffCommandHandler : ICommandHandler<PublishTariffCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public PublishTariffCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(PublishTariffCommand command, CancellationToken cancellationToken = default)
    {
        var draftTariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        Error.Throw().TariffNotFoundIfNull(draftTariff, command.TariffId);

        var realTariff = draftTariff.CopyAsReal();

        _tariffRepository.Add(realTariff);
        _tariffRepository.Delete(draftTariff);

        await _tariffRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}