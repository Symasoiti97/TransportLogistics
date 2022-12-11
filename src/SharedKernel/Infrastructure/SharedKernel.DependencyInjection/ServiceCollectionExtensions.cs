using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Application.Repositories;
using TL.SharedKernel.Infrastructure.DataAccess;
using TL.SharedKernel.Infrastructure.DependencyInjection.Factories;

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
}