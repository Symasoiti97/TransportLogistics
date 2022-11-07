using Application.Abstracts;
using Application.Abstracts.Repositories;
using Tariffs.Domain.AggregateTariff;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик для создания тарифа
/// </summary>
internal sealed class CreateTariffCommandHandler : ICommandHandler<CreateTariffCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTariffCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(CreateTariffCommand command, CancellationToken cancellationToken)
    {
        var tariffRepository = _unitOfWork.GetRepository<ITariffRepository>();

        var tariff = new Tariff(command.TariffId, command.ManagerProfileId);

        tariffRepository.Add(tariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}