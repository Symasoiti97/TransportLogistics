using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface IUserSessionRepository
{
    Task AddAsync(UserSession user, CancellationToken cancellationToken);
}