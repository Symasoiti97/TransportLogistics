using System.Text.Json;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;
using IRedisDatabase = StackExchange.Redis.IDatabase;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

internal sealed class UserSessionRepository(IRedisDatabase database) : IUserSessionRepository
{
    public async Task AddAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        var userSessionKey = BuildUserSessionKey(userSession.Id);
        var jsonData = JsonSerializer.Serialize(userSession);

        var refreshTokenIndexKey = BuildRefreshTokenIndexKey(userSession.RefreshToken);

        await database.StringSetAsync(userSessionKey, jsonData, TimeSpan.FromDays(30));
        await database.StringSetAsync(refreshTokenIndexKey, userSession.Id.ToString(), TimeSpan.FromDays(30));
    }

    private static string BuildUserSessionKey(Guid userSessionId) => $"UserSession:{userSessionId}";
    private static string BuildRefreshTokenIndexKey(string refreshToken) => $"RefreshTokenIndex:{refreshToken}";
}