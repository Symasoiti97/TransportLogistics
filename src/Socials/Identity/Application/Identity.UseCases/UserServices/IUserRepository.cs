using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface IUserRepository
{
    Task<User?> FindAsync(Email email, CancellationToken cancellationToken);
    Task AddAsync(User tariff, CancellationToken cancellationToken);
}