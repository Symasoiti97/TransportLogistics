using Application.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Tariffs.Application.TariffServices;

namespace Tariffs.Application.DependencyInjection;

public static class ServiceExtensions
{
    /// <summary>
    /// Registers the tariff services
    /// <list type="bullet">
    ///     <item>
    ///     Registers <see cref="ICommandHandler{TCommand}"/> as <see cref="ServiceLifetime.Transient"/>.<br/>
    ///     Available commands:
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
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddTariffApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<ICommandHandler<CreateTariffCommand>, CreateTariffCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffRouteCommand>, SaveTariffRouteCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffCargoCommand>, SaveTariffCargoCommandHandler>();
        services.AddTransient<ICommandHandler<SaveTariffPriceCommand>, SaveTariffPriceCommandHandler>();
        services.AddTransient<ICommandHandler<PublishTariffCommand>, PublishTariffCommandHandler>();

        return services;
    }
}