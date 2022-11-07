using Application.Abstracts;
using Application.Abstracts.Repositories;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Обработчик команды сохранения тарифа с параметрами груза
/// </summary>
internal sealed class SaveTariffCargoCommandHandler : ICommandHandler<SaveTariffCargoCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public SaveTariffCargoCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(SaveTariffCargoCommand command, CancellationToken cancellationToken = default)
    {
        var tariffRepository = _unitOfWork.GetRepository<ITariffRepository>();
        var tariff = await tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);
        if (tariff == null)
            throw new Exception("Tariff not found");

        tariff.SetCargoType(command.CargoType);
        tariff.SetContainerOwn(command.ContainerOwn);
        tariff.SetContainerSize(command.ContainerSize);

        tariffRepository.Update(tariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}