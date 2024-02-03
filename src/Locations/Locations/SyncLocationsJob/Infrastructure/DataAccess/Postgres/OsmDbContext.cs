using Microsoft.EntityFrameworkCore;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Neo4j.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres;

internal sealed class OsmDbContext : DbContext
{
    public OsmDbContext(DbContextOptions<OsmDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.DefaultTypeMapping<Node>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Node>();
        modelBuilder.Entity<TempLocation>();
    }
}