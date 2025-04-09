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
    : TokenRepository<UserRegisterViaEmailToken>(database, eventOptions, serializerOptions),
        IUserRegisterViaEmailTokenRepository
{
    public async Task<UserRegisterViaEmailToken?> FindAsync(Email email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenEmailIndexKey = BuildTokenEmailIndex(email);

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

        return JsonSerializer.Deserialize<UserRegisterViaEmailToken>((byte[]) jsonValue!)
               ?? throw new InvalidOperationException("Deserialize error.");
    }

    protected override void SetIndexes(ITransaction transaction, UserRegisterViaEmailToken token, string tokenKey)
    {
        _ = transaction.StringSetAsync(BuildTokenEmailIndex(token.Email), tokenKey, TimeSpan.FromDays(3));

        base.SetIndexes(transaction, token, tokenKey);
    }

    protected override string BuildTokenKey(Guid tokenId) => $"RequestEmailRegisterToken:{tokenId}";

    protected override string BuildTokenValueIndexKey(string tokenValue) =>
        $"RequestEmailRegisterTokenValue:{tokenValue}";

    private static string BuildTokenEmailIndex(Email email) => $"RequestEmailRegisterTokenEmailIndex:{email.Value}";
}