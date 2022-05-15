using Application.Abstracts;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик для создания тарифа
/// </summary>
internal class CreateTariffCommandHandler : ICommandHandler<CreateTariffCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public CreateTariffCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(CreateTariffCommand command, CancellationToken cancellationToken)
    {
        var tariff = new Tariff(command.TariffId, command.ManagerProfileId);

        await _tariffRepository.CreateAsync(tariff, cancellationToken);
    }
}