using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

public interface IUserRegisterViaEmailTokenRepository : ITokenRepository<UserRegisterViaEmailToken>
{
    Task<UserRegisterViaEmailToken?> FindAsync(Email email, CancellationToken cancellationToken);
}
