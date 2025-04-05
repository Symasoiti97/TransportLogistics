using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Application.Repositories;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.DependencyInjection;
using TL.SharedKernel.Infrastructure.DependencyInjection.Settings;
using TL.TransportLogistics.Tariffs.Application.UseCases.LocationServices;
using TL.TransportLogistics.Tariffs.Application.UseCases.TariffServices;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j;
using TL.TransportLogistics.Tariffs.Infrastructure.DataAccess.Neo4j.Queries;
using TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection.MockServices;

namespace TL.TransportLogistics.Tariffs.Infrastructure.DependencyInjection;

public static class ServiceExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>
    ///     Регестрирует <see cref="IUserContext" />, как <see cref="ServiceLifetime.Scoped" /> только mock для
    ///     ASPNETCORE_ENVIRONMENT=Development
    ///     </item>
    ///     <item>
    ///     Регестрирует <see cref="IUseCase{TCommand}" />, как <see cref="ServiceLifetime.Transient" />.<br />
    ///     Доступные команды:
    ///     <list type="bullet">
    ///         <item><see cref="CreateTariffCommand" /></item>
    ///         <item><see cref="SaveTariffRouteCommand" /></item>
    ///         <item><see cref="SaveTariffCargoEquipmentCommand" /></item>
    ///         <item><see cref="SaveTariffPriceCommand" /></item>
    ///         <item><see cref="PublishTariffCommand" /></item>
    ///     </list>
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
        services.AddTransient<IUseCaseHandler<CreateTariffCommand>, CreateTariffCommandHandler>();
        services.AddTransient<IUseCaseHandler<SaveTariffRouteCommand>, SaveTariffRouteCommandHandler>();
        services.AddTransient<IUseCaseHandler<SaveTariffCargoEquipmentCommand>, SaveTariffCargoCommandHandler>();
        services.AddTransient<IUseCaseHandler<SaveTariffPriceCommand>, SaveTariffPriceCommandHandler>();
        services.AddTransient<IUseCaseHandler<PublishTariffCommand>, PublishTariffCommandHandler>();
        services.AddTransient<IUseCaseHandler<GetTariffQuery, TariffView>, GetTariffQueryHandler>();

        services.AddNeo4JServices(neo4JSettings);

        services.AddTransient<ITariffRepository, TariffRepository>();
        services.AddTransient<ILocationRepository, LocationRepository>();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddTransient<IUserContext, StubUserContext>();
        }

        return services;
    }
}