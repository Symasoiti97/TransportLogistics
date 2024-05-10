using TL.SharedKernel.Application.Commands;

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
        var tariff = await _tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        tariff.SetAsReal();

        await _tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}