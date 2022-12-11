using System.Collections.Concurrent;
using Neo4jClient.Cypher;
using Neo4jClient.Transactions;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;

internal sealed class TariffDbContext
{
    private readonly ConcurrentQueue<Func<ICypherFluentQuery, Task>> _commands = new();
    private readonly ICypherGraphClientFactory _cypherGraphClientFactory;

    public TariffDbContext(ICypherGraphClientFactory cypherGraphClientFactory)
    {
        _cypherGraphClientFactory = cypherGraphClientFactory;
    }

    public async Task<TResult> ReadAsync<TResult>(
        Func<ICypherFluentQuery, Task<TResult>> query,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cypherGraphClient = await _cypherGraphClientFactory
            .GetCypherGraphClientAsync(cancellationToken)
            .ConfigureAwait(false);

        return await query(cypherGraphClient.Cypher).ConfigureAwait(false);
    }

    public void AddCommand(Func<ICypherFluentQuery, Task> command)
    {
        _commands.Enqueue(command);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cypherGraphClient = await _cypherGraphClientFactory
            .GetTransactionCypherGraphClientAsync(cancellationToken)
            .ConfigureAwait(false);

        using var beginTransaction = cypherGraphClient.BeginTransaction(TransactionScopeOption.Join);

        while (_commands.TryDequeue(out var command))
        {
            await command(cypherGraphClient.Cypher).ConfigureAwait(false);
        }

        await beginTransaction.CommitAsync().ConfigureAwait(false);
    }
}