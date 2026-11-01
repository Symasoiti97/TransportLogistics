using Neo4jClient.Cypher;

namespace TL.SharedKernel.Infrastructure.Neo4j;

/// <summary>
/// Factory that provides a Neo4j client
/// </summary>
public interface ICypherGraphClientFactory
{
    /// <summary>
    /// Get a Neo4j client
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Neo4j client</returns>
    Task<ICypherFluentQuery> GetCypherFluentQueryAsync(CancellationToken cancellationToken);
}
