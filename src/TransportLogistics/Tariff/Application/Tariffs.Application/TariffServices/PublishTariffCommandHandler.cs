using System.Transactions;
using Application.Abstracts;

namespace Tariffs.Application.TariffServices;

/// <summary>
/// Команда публикации тарифа
/// Переводит тариф из черновика в действующий (Создает копию)
/// </summary>
internal class PublishTariffCommandHandler : ICommandHandler<PublishTariffCommand>
{
    private readonly ITariffRepository _tariffRepository;

    public PublishTariffCommandHandler(ITariffRepository tariffRepository)
    {
        _tariffRepository = tariffRepository;
    }

    public async Task HandleAsync(PublishTariffCommand command, CancellationToken cancellationToken = default)
    {
        var tariff = await _tariffRepository.FindAsync(command.TariffId, cancellationToken);
        if (tariff == null)
            throw new Exception("Tariff not found");

        var realTariff = tariff.CopyAsReal();

        using var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions {IsolationLevel = IsolationLevel.RepeatableRead},
            TransactionScopeAsyncFlowOption.Enabled);

        await _tariffRepository.CreateAsync(realTariff, cancellationToken);
        await _tariffRepository.DeleteDraftAsync(tariff.Id, cancellationToken);

        transaction.Complete();
    }
}