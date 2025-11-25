using Microsoft.EntityFrameworkCore;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.DataAccess.Postgres.Extensions;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Configuration;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Repositories;

internal sealed class UserRepository(IdentityDbContext identityDbContext) : IUserRepository
{
    public Task<User?> FindAsync(Email email, CancellationToken cancellationToken)
        => identityDbContext.Set<User>().SingleOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<User?> FindAsync(PhoneNumber phoneNumber, CancellationToken cancellationToken)
        => identityDbContext.Set<User>()
            .SingleOrDefaultAsync(
                user => user.PhoneNumber == phoneNumber,
                cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        identityDbContext.Set<User>().Add(user);

        try
        {
            await identityDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.IsPostgresUniqueConstraintViolation(UserConfiguration.UniqueEmailConstraintName))
        {
            throw new Conflict().WithDetails("User already exists.", exception);
        }
    }
}
