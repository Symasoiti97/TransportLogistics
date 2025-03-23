using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Configuration;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres;

internal sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}

// TODO: Temparary solution. See https://learn.microsoft.com/en-gb/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli
internal sealed class BloggingContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=tl-identity;Username=postgres;Password=postgres");

        return new IdentityDbContext(optionsBuilder.Options);
    }
}