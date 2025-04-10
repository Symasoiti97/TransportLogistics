using System.Text.Json;
using TL.Socials.Identity.Application.UseCases.UserSessionServices;
using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;
using IRedisDatabase = StackExchange.Redis.IDatabase;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

internal sealed class UserSessionRepository(IRedisDatabase database) : IUserSessionRepository
{
    public async Task SaveAsync(UserSession userSession, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userSessionKey = BuildUserSessionKey(userSession.Id);
        var jsonData = JsonSerializer.Serialize(userSession);

        var refreshTokenIndexKey = BuildRefreshTokenIndexKey(userSession.RefreshToken);

        var transaction = database.CreateTransaction();

        _ = transaction.StringSetAsync(userSessionKey, jsonData, TimeSpan.FromDays(30));
        _ = transaction.StringSetAsync(refreshTokenIndexKey, userSessionKey, TimeSpan.FromDays(30));

        await transaction.ExecuteAsync();
    }

    public async Task<UserSession?> FindAsync(string refreshToken, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var refreshTokenIndexKey = BuildRefreshTokenIndexKey(refreshToken);

        var userSessionKey = await database.StringGetAsync(refreshTokenIndexKey).ConfigureAwait(false);
        if (!userSessionKey.HasValue)
        {
            return null;
        }

        var jsonValue = await database.StringGetAsync((string) userSessionKey!).ConfigureAwait(false);
        if (!jsonValue.HasValue)
        {
            return null;
        }

        var userSession = JsonSerializer.Deserialize<UserSession>((byte[]) jsonValue!)
                          ?? throw new InvalidOperationException("Deserialize error.");

        if (userSession.RefreshToken != refreshToken)
        {
            await database.KeyDeleteAsync(refreshTokenIndexKey);
            return null;
        }

        return userSession;
    }

    private static string BuildUserSessionKey(Guid userSessionId) => $"UserSession:{userSessionId}";
    private static string BuildRefreshTokenIndexKey(string refreshToken) => $"RefreshTokenIndex:{refreshToken}";
}