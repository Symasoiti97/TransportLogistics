using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
{
    public void Configure(EntityTypeBuilder<Terminal> builder)
    {
        builder.ToTable("terminals");
        builder.HasKey(terminal => terminal.OsmId);
        builder.Property(terminal => terminal.OsmId).HasColumnName("osm_id");
        builder.Property(terminal => terminal.OsmType).HasColumnName("osm_type");
        builder.Property(terminal => terminal.Name).HasColumnName("name");
        builder.Property(terminal => terminal.NameEn).HasColumnName("name_en");
        builder.Property(terminal => terminal.NameRu).HasColumnName("name_ru");
        builder.Property(terminal => terminal.Building).HasColumnName("building");
        builder.Property(terminal => terminal.Amenity).HasColumnName("amenity");
        builder.Property(terminal => terminal.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(terminal => terminal.Geom).HasColumnName("geom");
        builder.Property(terminal => terminal.Centroid).HasColumnName("centroid");
    }
}
