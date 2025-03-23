using Microsoft.EntityFrameworkCore;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Repositories;

internal sealed class UserRepository(IdentityDbContext identityDbContext) : IUserRepository
{
    public Task<User?> FindAsync(Email email, CancellationToken cancellationToken)
    {
        return identityDbContext.Set<User>().SingleOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        identityDbContext.Set<User>().Add(user);

        return identityDbContext.SaveChangesAsync(cancellationToken);
    }
}