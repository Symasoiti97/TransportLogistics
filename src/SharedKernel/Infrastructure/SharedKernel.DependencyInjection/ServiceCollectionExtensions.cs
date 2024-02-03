using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Application.Repositories;
using TL.SharedKernel.Infrastructure.DataAccess;
using TL.SharedKernel.Infrastructure.DependencyInjection.Factories;
using TL.SharedKernel.Infrastructure.DependencyInjection.Settings;
using TL.SharedKernel.Infrastructure.Neo4j;

namespace TL.SharedKernel.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>Регестрирует <see cref="IUnitOfWork"/>, как <see cref="ServiceLifetime.Scoped"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddTransient<IRepositoryFactory, RepositoryFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    /// <summary>
    /// Регистрация сервисов для работы с neo4j
    /// <list type="bullet">
    ///     <item>Регестрирует <see cref="ICypherGraphClientFactory"/>, как <see cref="ServiceLifetime.Singleton"/>.</item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="neo4JSettings"></param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddNeo4JServices(this IServiceCollection services, INeo4JSettings neo4JSettings)
    {
        services.AddSingleton(neo4JSettings);
        services.AddSingleton<ICypherGraphClientFactory, CypherGraphClientFactory>();

        return services;
    }
}