using System.Collections.Concurrent;
using System.Transactions;
using EnsureThat;
using TL.SharedKernel.Application.Repositories;

namespace TL.SharedKernel.Infrastructure.DataAccess;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, IRepository> _repositories = new();
    private readonly IRepositoryFactory _repositoryFactory;

    public UnitOfWork(IRepositoryFactory repositoryFactory)
    {
        EnsureArg.IsNotNull(repositoryFactory, nameof(repositoryFactory));

        _repositoryFactory = repositoryFactory;
    }

    public TRepository GetRepository<TRepository>() where TRepository : IRepository
    {
        var repository = _repositories.GetOrAdd(typeof(TRepository), _repositoryFactory.Get<TRepository>());

        return (TRepository) repository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted},
            TransactionScopeAsyncFlowOption.Enabled);

        foreach (var repository in _repositories.Values.OfType<IWriteRepository>())
        {
            await repository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        transaction.Complete();
    }
}