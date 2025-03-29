using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface IUserRegisterViaEmailTokenRepository
{
    Task AddAsync(UserRegisterViaEmailToken token, CancellationToken cancellationToken);
    Task<UserRegisterViaEmailToken?> FindAsync(Email email, CancellationToken cancellationToken);
}