using Microsoft.Extensions.Logging;
using Neo4jClient;
using Neo4jClient.Transactions;
using Nito.AsyncEx;
using TL.SharedKernel.Infrastructure.DependencyInjection.Settings;
using TL.SharedKernel.Infrastructure.Neo4j;

namespace TL.SharedKernel.Infrastructure.DependencyInjection.Factories;

internal class CypherGraphClientFactory : ICypherGraphClientFactory
{
    private readonly AsyncLazy<BoltGraphClient> _cypherGraphClientLazy;

    public CypherGraphClientFactory(INeo4JSettings neo4JSettings, ILogger<CypherGraphClientFactory> logger)
    {
        _cypherGraphClientLazy = new AsyncLazy<BoltGraphClient>(
            async () =>
            {
                var boltGraphClient = new BoltGraphClient(
                    neo4JSettings.Uri,
                    neo4JSettings.UserName,
                    neo4JSettings.Password);

                boltGraphClient.OperationCompleted += OnCypherGraphClientOnOperationCompleted;
                await boltGraphClient.ConnectAsync().ConfigureAwait(false);

                return boltGraphClient;

                void OnCypherGraphClientOnOperationCompleted(object _, OperationCompletedEventArgs eventArgs)
                {
                    if (eventArgs.HasException)
                    {
                        logger.LogError(
                            eventArgs.Exception,
                            "Neo4j exception. Database: {Database} QueryText: {QueryText}",
                            eventArgs.Database,
                            eventArgs.QueryText);
                    }
                    else
                    {
                        logger.LogDebug("OperationCompleted. QueryText: {QueryText}", eventArgs.QueryText);
                    }
                }
            });
    }

    public async Task<ICypherGraphClient> GetCypherGraphClientAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _cypherGraphClientLazy.Task.ConfigureAwait(false);
    }

    public async Task<ITransactionalGraphClient> GetTransactionCypherGraphClientAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _cypherGraphClientLazy.Task.ConfigureAwait(false);
    }
}