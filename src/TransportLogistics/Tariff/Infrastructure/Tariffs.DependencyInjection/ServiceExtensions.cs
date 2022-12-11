using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Application.Repositories;
using TL.SharedKernel.Infrastructure.DependencyInjection;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Queries;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.Factories;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.MockServices;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.Settings;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection;

public static class ServiceExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>Регестрирует <see cref="IUnitOfWork"/>, как <see cref="ServiceLifetime.Scoped"/></item>
    ///     <item>Регестрирует <see cref="IUserContext"/>, как <see cref="ServiceLifetime.Scoped"/> только mock для ASPNETCORE_ENVIRONMENT=Development</item>
    ///     <item>
    ///     Регестрирует <see cref="ICommandHandler{TCommand}"/>, как <see cref="ServiceLifetime.Transient"/>.<br/>
    ///     Доступные команды:
    ///         <list type="bullet">
    ///             <item><see cref="CreateTariffCommand"/></item>
    ///             <item><see cref="SaveTariffRouteCommand"/></item>
    ///             <item><see cref="SaveTariffCargoEquipmentCommand"/></item>
    ///             <item><see cref="SaveTariffPriceCommand"/></item>
    ///             <item><see cref="PublishTariffCommand"/></item>
    ///         </list>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="neo4JSettings">Neo4j connection settings</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddTariffServices(
        this IServiceCollection services,
        INeo4JSettings neo4JSettings)
    {
        services.AddTransient<ICommandHandler<CreateTariffCommand>, CreateTariffCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffRouteCommand>, SaveTariffRouteCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffCargoEquipmentCommand>, SaveTariffCargoCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffPriceCommand>, SaveTariffPriceCommandHandler>();
        services.AddTransient<ICommandHandler<PublishTariffCommand>, PublishTariffCommandHandler>();
        services.AddTransient<IQueryHandler<GetTariffQuery, TariffView>, GetTariffQueryHandler>();

        services.AddSingleton(neo4JSettings);
        services.AddSingleton<ICypherGraphClientFactory, CypherGraphClientFactory>();

        services.AddScoped<TariffDbContext>();
        services.AddTransient<ITariffRepository, TariffRepository>();
        services.AddTransient<ILocationRepository, LocationRepository>();

        services.AddUnitOfWork();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddTransient<IUserContext, StubUserContext>();
        }

        return services;
    }
}