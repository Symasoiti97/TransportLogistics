using Application.Abstracts;
using Application.Abstracts.Repositories;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
internal sealed class PublishTariffCommandHandler : ICommandHandler<PublishTariffCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public PublishTariffCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(PublishTariffCommand command, CancellationToken cancellationToken = default)
    {
        var tariffRepository = _unitOfWork.GetRepository<ITariffRepository>();

        var draftTariff = await tariffRepository.FindAsync(command.TariffId, cancellationToken).ConfigureAwait(false);
        if (draftTariff == null)
            throw new Exception("Tariff not found");

        var realTariff = draftTariff.CopyAsReal();

        tariffRepository.Add(realTariff);
        tariffRepository.Delete(draftTariff);

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}