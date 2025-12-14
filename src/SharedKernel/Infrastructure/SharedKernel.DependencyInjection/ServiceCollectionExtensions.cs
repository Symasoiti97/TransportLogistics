using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Infrastructure.DependencyInjection.Factories;
using TL.SharedKernel.Infrastructure.DependencyInjection.Settings;
using TL.SharedKernel.Infrastructure.Neo4j;

namespace TL.SharedKernel.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNeo4JServices(this IServiceCollection services, INeo4JSettings neo4JSettings)
    {
        services.AddSingleton(neo4JSettings);
        services.AddSingleton<ICypherGraphClientFactory, CypherGraphClientFactory>();

        return services;
    }
}
