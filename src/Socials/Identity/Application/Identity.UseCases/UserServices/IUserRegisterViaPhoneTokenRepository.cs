using TL.Socials.Identity.Business.Aggregates.TokenAggregate;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface IUserLoginViaPhoneTokenRepository : ITokenRepository<UserLoginViaPhoneToken>
{
    Task<UserLoginViaPhoneToken?> FindAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken);
}