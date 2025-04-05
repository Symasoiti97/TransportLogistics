using System.Text.Json;
using StackExchange.Redis;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

internal sealed class UserRegisterViaEmailTokenRepository(
    IDatabase database,
    EventProcessorOptions eventOptions,
    JsonSerializerOptions serializerOptions)
    : IUserRegisterViaEmailTokenRepository
{
    public async Task AddAsync(UserRegisterViaEmailToken token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenKey = BuildTokenKey(token.Id);
        var tokenEmailIndexKey = BuildTokenEmailIndex(token.Email);
        var jsonData = JsonSerializer.SerializeToUtf8Bytes(token);

        await database.StringSetAsync(tokenKey, jsonData, TimeSpan.FromDays(3));
        await database.StringSetAsync(tokenEmailIndexKey, tokenKey, TimeSpan.FromDays(3));

        foreach (var tokenEvent in token.Events)
        {
            await database.StreamAddAsync(
                eventOptions.StreamName,
                [
                    new NameValueEntry("event", JsonSerializer.Serialize(tokenEvent, serializerOptions))
                ]);
        }
    }

    public async Task<UserRegisterViaEmailToken?> FindAsync(Email email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenEmailIndexKey = BuildTokenEmailIndex(email);

        var tokenKeyValue = await database.StringGetAsync(tokenEmailIndexKey).ConfigureAwait(false);
        if (!tokenKeyValue.HasValue)
        {
            return null;
        }

        var jsonValue = await database.StringGetAsync((string) tokenKeyValue!).ConfigureAwait(false);
        if (!jsonValue.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserRegisterViaEmailToken>((byte[]) jsonValue!)
               ?? throw new InvalidOperationException("Deserialize error.");
    }

    private static string BuildTokenKey(Guid tokenId) => $"RequestEmailRegisterToken:{tokenId}";
    private static string BuildTokenEmailIndex(Email email) => $"RequestEmailRegisterTokenEmailIndex:{email.Value}";
}