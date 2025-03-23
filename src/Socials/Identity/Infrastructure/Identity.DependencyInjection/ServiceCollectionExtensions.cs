using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Repositories;

namespace TL.Socials.Identity.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>
    ///     Регестрирует <see cref="ICommandHandler{TCommand}" />, как <see cref="ServiceLifetime.Transient" />.<br />
    ///     Доступные команды:
    ///     <list type="bullet">
    ///         <item><see cref="RegisterUserWithPasswordCommand" /></item>
    ///     </list>
    ///     </item>
    ///     <item>
    ///     Регестрирует <see cref="IQueryHandler{TQuery, TResult}" />, как <see cref="ServiceLifetime.Transient" />.<br />
    ///     Доступные queries:
    ///     <list type="bullet">
    ///         <item><see cref="LoginUserByPasswordCommand" /></item>
    ///     </list>
    ///     </item>
    ///     <item>
    ///     Регестрирует <see cref="IUserRepository" />, как <see cref="ServiceLifetime.Transient" />
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="pgConnectionString">Строка подключения к postgres</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, string pgConnectionString)
    {
        services
            .AddTransient<ICommandHandler<RegisterUserWithPasswordCommand>, RegisterUserWithPasswordCommandHandler>();
        services
            .AddTransient<IQueryHandler<LoginUserByPasswordCommand, UserTokens>, LoginUserByPasswordCommandHandler>();

        services.AddTransient<IUserRepository, UserRepository>();

        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(pgConnectionString));

        return services;
    }
}