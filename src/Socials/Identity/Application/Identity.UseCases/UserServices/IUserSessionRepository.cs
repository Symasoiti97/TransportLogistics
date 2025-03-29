using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface IUserSessionRepository
{
    Task SaveAsync(UserSession userSession, CancellationToken cancellationToken);
    Task<UserSession?> FindAsync(string refreshToken, CancellationToken cancellationToken);
}