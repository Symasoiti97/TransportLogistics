using TL.SharedKernel.Application.Commands;
using TL.TransportLogistics.Tariffs.Business.Aggregates.AggregateTariff;

namespace TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;

/// <summary>
/// Обработчик для создания тарифа
/// </summary>
internal sealed class CreateTariffCommandHandler(ITariffRepository tariffRepository)
    : IUseCaseHandler<CreateTariffCommand>
{
    public async Task HandleAsync(CreateTariffCommand command, CancellationToken cancellationToken)
    {
        var tariff = Tariff.Create(command.TariffId, command.ManagerProfileId);

        await tariffRepository.AddAsync(tariff, cancellationToken);
    }
}
