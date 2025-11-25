using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

public interface ITokenRepository<TToken> where TToken : Token
{
    Task SaveAsync(TToken token, CancellationToken cancellationToken);
    Task<TToken?> FindAsync(string tokenValue, CancellationToken cancellationToken);
    Task<TToken?> FindAsync(Guid tokenId, CancellationToken cancellationToken);
}
