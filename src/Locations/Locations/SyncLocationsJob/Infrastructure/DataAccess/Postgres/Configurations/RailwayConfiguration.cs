using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class RailwayConfiguration : IEntityTypeConfiguration<Railway>
{
    public void Configure(EntityTypeBuilder<Railway> builder)
    {
        builder.ToTable("railways");
        builder.HasKey(railway => railway.OsmId);
        builder.Property(railway => railway.OsmId).HasColumnName("osm_id");
        builder.Property(railway => railway.OsmType).HasColumnName("osm_type");
        builder.Property(railway => railway.Name).HasColumnName("name");
        builder.Property(railway => railway.NameEn).HasColumnName("name_en");
        builder.Property(railway => railway.NameRu).HasColumnName("name_ru");
        builder.Property(railway => railway.RailwayType).HasColumnName("railway");
        builder.Property(railway => railway.Station).HasColumnName("station");
        builder.Property(railway => railway.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(railway => railway.Geom).HasColumnName("geom");
    }
}
