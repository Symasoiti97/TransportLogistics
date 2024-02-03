using Neo4jClient;
using Neo4jClient.Transactions;

namespace TL.SharedKernel.Infrastructure.Neo4j;

/// <summary>
/// Фабрика получения клиента для работы с neo4j
/// </summary>
public interface ICypherGraphClientFactory
{
    /// <summary>
    /// Получить клиента для работы c neo4j
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Клиент для работы с neo4j</returns>
    Task<ICypherGraphClient> GetCypherGraphClientAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Получить клиента для работы c neo4j
    /// </summary>
    /// <param name="cancellationToken">Токе отмены</param>
    /// <returns>Клиент для работы с neo4j</returns>
    Task<ITransactionalGraphClient> GetTransactionCypherGraphClientAsync(CancellationToken cancellationToken);
}