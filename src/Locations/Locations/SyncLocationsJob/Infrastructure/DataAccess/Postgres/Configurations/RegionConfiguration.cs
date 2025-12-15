using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("regions");
        builder.HasKey(region => region.OsmId);
        builder.Property(region => region.OsmId).HasColumnName("osm_id");
        builder.Property(region => region.Name).HasColumnName("name");
        builder.Property(region => region.NameEn).HasColumnName("name_en");
        builder.Property(region => region.NameRu).HasColumnName("name_ru");
        builder.Property(region => region.AdminLevel).HasColumnName("admin_level");
        builder.Property(region => region.PostalCode).HasColumnName("postal_code");
        builder.Property(region => region.Population).HasColumnName("population");
        builder.Property(region => region.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(region => region.Geom).HasColumnName("geom");
        builder.Property(region => region.Centroid).HasColumnName("centroid");
    }
}
