using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class PortConfiguration : IEntityTypeConfiguration<Port>
{
    public void Configure(EntityTypeBuilder<Port> builder)
    {
        builder.ToTable("ports");
        builder.HasKey(port => port.OsmId);
        builder.Property(port => port.OsmId).HasColumnName("osm_id");
        builder.Property(port => port.OsmType).HasColumnName("osm_type");
        builder.Property(port => port.Name).HasColumnName("name");
        builder.Property(port => port.NameEn).HasColumnName("name_en");
        builder.Property(port => port.NameRu).HasColumnName("name_ru");
        builder.Property(port => port.Harbour).HasColumnName("harbour");
        builder.Property(port => port.SeamarkType).HasColumnName("seamark_type");
        builder.Property(port => port.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(port => port.Geom).HasColumnName("geom");
        builder.Property(port => port.Centroid).HasColumnName("centroid");
    }
}
