using Application.Abstracts;
using Application.Abstracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Tariffs.Application.LocationServices;
using Tariffs.Application.TariffServices;
using Tariffs.Infrastructure.DataAccess.Neo4j;
using Tariffs.Infrastructure.DependencyInjection.Factories;
using Tariffs.Infrastructure.DependencyInjection.MockServices;
using Tariffs.Infrastructure.DependencyInjection.Settings;
using TL.SharedKernel.Infrastructure.DependencyInjection;

namespace Tariffs.Infrastructure.DependencyInjection;

public static class ServiceExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>Регестрирует <see cref="IUnitOfWork"/>, как <see cref="ServiceLifetime.Scoped"/></item>
    ///     <item>Регестрирует <see cref="IUserContext"/>, как <see cref="ServiceLifetime.Scoped"/> только mock для ASPNETCORE_ENVIRONMENT=Development</item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="neo4JSettings">Neo4j connection settings</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddTariffInfrastructureServices(this IServiceCollection services, INeo4JSettings neo4JSettings)
    {
        services.AddSingleton(neo4JSettings);
        services.AddSingleton<ICypherGraphClientFactory, CypherGraphClientFactory>();

        services.AddScoped<TariffDbContext>();
        services.AddTransient<ITariffRepository, TariffRepository>();
        services.AddTransient<ILocationRepository, LocationRepository>();

        services.AddUnitOfWork();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddTransient<IUserContext, MockUserContext>();
        }

        return services;
    }
}