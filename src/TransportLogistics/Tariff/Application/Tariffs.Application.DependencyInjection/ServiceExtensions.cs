using Application.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Tariffs.Application.TariffServices;

namespace Tariffs.Application.DependencyInjection;

public static class ServiceExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>
    ///     Регестрирует <see cref="ICommandHandler{TCommand}"/>, как <see cref="ServiceLifetime.Transient"/>.<br/>
    ///     Доступные команды:
    ///         <list type="bullet">
    ///             <item><see cref="CreateTariffCommand"/></item>
    ///             <item><see cref="SaveTariffRouteCommand"/></item>
    ///             <item><see cref="SaveTariffCargoCommand"/></item>
    ///             <item><see cref="SaveTariffPriceCommand"/></item>
    ///             <item><see cref="PublishTariffCommand"/></item>
    ///         </list>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddTariffServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<CreateTariffCommand>, CreateTariffCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffRouteCommand>, SaveTariffRouteCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffCargoCommand>, SaveTariffCargoCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffPriceCommand>, SaveTariffPriceCommandHandler>();
        services.AddTransient<ICommandHandler<PublishTariffCommand>, PublishTariffCommandHandler>();

        return services;
    }
}