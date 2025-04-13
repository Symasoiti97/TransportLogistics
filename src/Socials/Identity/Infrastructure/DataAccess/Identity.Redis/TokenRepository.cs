using System.Text.Json;
using StackExchange.Redis;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

internal abstract class TokenRepository<TToken>(
    IDatabase database,
    EventProcessorOptions eventOptions,
    JsonSerializerOptions serializerOptions)
    where TToken : Token
{
    protected readonly IDatabase Database = database;

    public async Task SaveAsync(TToken token, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var jsonData = JsonSerializer.SerializeToUtf8Bytes(token, serializerOptions);

        var transaction = Database.CreateTransaction();

        var tokenKey = BuildTokenKey(token.Id);
        var expiration = token.ExpiresAt - DateTimeOffset.UtcNow;
        _ = transaction.StringSetAsync(tokenKey, jsonData, expiration);

        SetIndexes(transaction, token, tokenKey, expiration);

        foreach (var tokenEvent in token.Events)
        {
            _ = transaction.StreamAddAsync(
                eventOptions.StreamName,
                [
                    new NameValueEntry("event", JsonSerializer.Serialize(tokenEvent, serializerOptions))
                ]);
        }

        await transaction.ExecuteAsync();
    }

    public async Task<TToken?> FindAsync(Guid tokenId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenKey = BuildTokenKey(tokenId);

        var jsonValue = await Database.StringGetAsync(tokenKey).ConfigureAwait(false);
        if (!jsonValue.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<TToken>((byte[]) jsonValue!)
               ?? throw new InvalidOperationException("Deserialize error.");
    }

    public async Task<TToken?> FindAsync(string tokenValue, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tokenValueKey = BuildTokenValueIndexKey(tokenValue);

        var tokenKeyValue = await Database.StringGetAsync(tokenValueKey).ConfigureAwait(false);
        if (!tokenKeyValue.HasValue)
        {
            return null;
        }

        var jsonValue = await Database.StringGetAsync((string) tokenKeyValue!).ConfigureAwait(false);
        if (!jsonValue.HasValue)
        {
            return null;
        }

        return JsonSerializer.Deserialize<TToken>((byte[]) jsonValue!)
               ?? throw new InvalidOperationException("Deserialize error.");
    }

    protected abstract string BuildTokenKey(Guid tokenId);
    protected abstract string BuildTokenValueIndexKey(string tokenValue);

    protected virtual void SetIndexes(ITransaction transaction, TToken token, string tokenKey, TimeSpan expiration)
    {
        var tokenValueIndexKey = BuildTokenValueIndexKey(token.Value);
        _ = transaction.StringSetAsync(tokenValueIndexKey, tokenKey, expiration);
    }
}