using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Repositories;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis;
using TL.Socials.Identity.Infrastructure.Services;
using TL.Socials.Identity.Infrastructure.Services.Options;
using IRedisDatabase = StackExchange.Redis.IDatabase;

namespace TL.Socials.Identity.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрация сервисов тарифа
    /// <list type="bullet">
    ///     <item>
    ///     Регестрирует <see cref="IUseCaseHandler{TUseCase}" />, как <see cref="ServiceLifetime.Transient" />.<br />
    ///     Доступные команды:
    ///     <list type="bullet">
    ///         <item><see cref="RequestUserRegisterViaEmailCommand" /></item>
    ///         <item><see cref="RegisterUserViaEmailCommand" /></item>
    ///         <item><see cref="LoginUserViaEmailCommand" /></item>
    ///     </list>
    ///     </item>
    ///     <item>
    ///     Регестрирует <see cref="IUserRepository" />, как <see cref="ServiceLifetime.Transient" />
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="jwtTokenOptions">JWT token options</param>
    /// <param name="pgConnectionString">Строка подключения к postgres</param>
    /// <param name="redisConnectionString">Строка подключения к redis</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        JwtTokenOptions jwtTokenOptions,
        string pgConnectionString,
        string redisConnectionString)
    {
        services
            .AddTransient<IUseCaseHandler<RequestUserRegisterViaEmailCommand, bool>,
                RequestUserRegisterViaEmailCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<RegisterUserViaEmailCommand>, RegisterUserWithPasswordCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<LoginUserViaEmailCommand, UserTokens>, LoginUserViaEmailCommandHandler>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserSessionRepository, UserSessionRepository>();
        services.AddTransient<IUserRegisterViaEmailTokenRepository, UserRegisterViaEmailTokenRepository>();
        services.AddSingleton(jwtTokenOptions);
        services.AddTransient<ITokenGenerator, TokenGenerator>();

        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(pgConnectionString));

        services.AddSingleton<IRedisDatabase>(_ => ConnectionMultiplexer.Connect(redisConnectionString).GetDatabase());

        return services;
    }
}