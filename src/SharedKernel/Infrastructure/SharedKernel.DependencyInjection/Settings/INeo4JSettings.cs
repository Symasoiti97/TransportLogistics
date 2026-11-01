namespace TL.SharedKernel.Infrastructure.DependencyInjection.Settings;

/// <summary>
/// Neo4j connection settings
/// </summary>
public interface INeo4JSettings
{
    /// <summary>
    /// Address
    /// </summary>
    Uri Uri { get; }

    /// <summary>
    /// User name
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// Password
    /// </summary>
    string Password { get; }
}
