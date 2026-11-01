using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.TokenServices;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Application.UseCases.UserSessionServices;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Repositories;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis;
using TL.Socials.Identity.Infrastructure.DependencyInjection.Stubs;
using TL.Socials.Identity.Infrastructure.Services;
using TL.Socials.Identity.Infrastructure.Services.Options;
using IRedisDatabase = StackExchange.Redis.IDatabase;

namespace TL.Socials.Identity.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the identity services
    /// <list type="bullet">
    ///     <item>
    ///     Registers <see cref="IUseCaseHandler{TUseCase}" /> as <see cref="ServiceLifetime.Transient" />.<br />
    ///     Available commands:
    ///     <list type="bullet">
    ///         <item><see cref="RequestUserRegisterViaEmailCommand" /></item>
    ///         <item><see cref="RegisterUserViaEmailCommand" /></item>
    ///         <item><see cref="LoginUserViaEmailCommand" /></item>
    ///     </list>
    ///     </item>
    ///     <item>
    ///     Registers <see cref="IUserRepository" /> as <see cref="ServiceLifetime.Transient" />
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="jwtTokenOptions">JWT token options</param>
    /// <param name="pgConnectionString">Postgres connection string</param>
    /// <param name="redisConnectionString">Redis connection string</param>
    /// <param name="useStubs">Whether to use stubs (for testing)</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        JwtTokenOptions jwtTokenOptions,
        string pgConnectionString,
        string redisConnectionString,
        bool useStubs = false)
    {
        if (useStubs)
        {
            services.AddTransient<RegisterUserViaEmailCommandHandler>();
            services
                .AddTransient<IUseCaseHandler<RegisterUserViaEmailCommand>, StubRegisterUserViaEmailCommandHandler>();
            services.AddTransient<RegisterUserViaPhoneCommandHandler>();
            services
                .AddTransient<IUseCaseHandler<RegisterUserViaPhoneCommand>, StubRegisterUserViaPhoneCommandHandler>();
        }
        else
        {
            services
                .AddTransient<IUseCaseHandler<RegisterUserViaEmailCommand>, RegisterUserViaEmailCommandHandler>();
        }

        services
            .AddTransient<IUseCaseHandler<RequestUserRegisterViaEmailCommand, bool>,
                RequestUserRegisterViaEmailCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<LoginUserViaEmailCommand, UserTokens>, LoginUserViaEmailCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<RefreshAuthTokenCommand, UserTokens>, RefreshAuthTokenCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<UserRegisterViaEmailTokenCreated>,
                SendRequestUserRegisterEmailEventHandler>();
        services
            .AddTransient<IUseCaseHandler<RequestUserLoginViaPhoneCommand>, RequestUserLoginViaPhoneCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<RegisterUserViaPhoneCommand>, RegisterUserViaPhoneCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<LoginUserViaPhoneCommand, UserTokens>, LoginUserViaPhoneCommandHandler>();
        services
            .AddTransient<IUseCaseHandler<UserLoginViaPhoneTokenCreated>, SendRequestUserLoginViaPhoneEventHandler>();

        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IUserSessionRepository, UserSessionRepository>();
        services.AddTransient<IUserRegisterViaEmailTokenRepository, UserRegisterViaEmailTokenRepository>();
        services.AddTransient<IUserLoginViaPhoneTokenRepository, UserLoginViaPhoneTokenRepository>();
        services.AddSingleton(jwtTokenOptions);
        services.AddTransient<IAuthTokenGenerator, AuthTokenGenerator>();

        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(pgConnectionString));

        services.AddSingleton<IRedisDatabase>(_ => ConnectionMultiplexer.Connect(redisConnectionString).GetDatabase());

        return services;
    }
}
