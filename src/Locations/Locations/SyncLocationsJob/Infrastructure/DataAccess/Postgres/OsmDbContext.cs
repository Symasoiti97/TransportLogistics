using Microsoft.EntityFrameworkCore;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;

internal sealed class OsmDbContext : DbContext
{
    public OsmDbContext(DbContextOptions<OsmDbContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(OsmDbContext).Assembly);
}
