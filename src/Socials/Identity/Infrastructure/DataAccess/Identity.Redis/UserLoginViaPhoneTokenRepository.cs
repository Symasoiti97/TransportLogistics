using System.Text.Json;
using StackExchange.Redis;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

internal sealed class UserLoginViaPhoneTokenRepository(
    IDatabase database,
    EventProcessorOptions eventOptions,
    JsonSerializerOptions serializerOptions)
    : TokenRepository<UserLoginViaPhoneToken>(database, eventOptions, serializerOptions),
        IUserLoginViaPhoneTokenRepository
{
    public async Task<UserLoginViaPhoneToken?> FindAsync(
        PhoneNumber phoneNumber,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenEmailIndexKey = BuildTokenPhoneIndexKey(phoneNumber);

        var tokenKeyValue = await Database.StringGetAsync(tokenEmailIndexKey).ConfigureAwait(false);
        if (!tokenKeyValue.HasValue)
        {
            return null;
        }

        var jsonValue = await Database.StringGetAsync((string) tokenKeyValue!).ConfigureAwait(false);
        if (!jsonValue.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserLoginViaPhoneToken>((byte[]) jsonValue!)
               ?? throw new InvalidOperationException("Deserialize error.");
    }

    protected override void SetIndexes(ITransaction transaction, UserLoginViaPhoneToken token, string tokenKey)
    {
        _ = transaction.StringSetAsync(BuildTokenPhoneIndexKey(token.PhoneNumber), tokenKey, TimeSpan.FromDays(3));

        base.SetIndexes(transaction, token, tokenKey);
    }

    protected override string BuildTokenKey(Guid tokenId) => $"UserLoginViaPhoneToken:{tokenId}";

    protected override string BuildTokenValueIndexKey(string tokenValue) => $"UserLoginViaPhoneTokenValue:{tokenValue}";

    private static string BuildTokenPhoneIndexKey(PhoneNumber phoneNumber)
        => $"UserLoginViaPhoneTokenPhoneIndex:{phoneNumber.Value}";
}