using TL.SharedKernel.Application.Commands;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
internal sealed class PublishTariffCommandHandler(ITariffRepository tariffRepository)
    : IUseCaseHandler<PublishTariffCommand>
{
    public async Task HandleAsync(PublishTariffCommand command, CancellationToken cancellationToken)
    {
        var tariff = await tariffRepository.GetAsync(command.TariffId, cancellationToken).ConfigureAwait(false);

        tariff.SetAsReal();

        await tariffRepository.UpdateAsync(tariff, cancellationToken);
    }
}