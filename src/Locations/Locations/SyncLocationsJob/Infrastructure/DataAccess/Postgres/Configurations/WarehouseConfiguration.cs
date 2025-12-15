using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");
        builder.HasKey(warehouse => warehouse.OsmId);
        builder.Property(warehouse => warehouse.OsmId).HasColumnName("osm_id");
        builder.Property(warehouse => warehouse.OsmType).HasColumnName("osm_type");
        builder.Property(warehouse => warehouse.Name).HasColumnName("name");
        builder.Property(warehouse => warehouse.NameEn).HasColumnName("name_en");
        builder.Property(warehouse => warehouse.NameRu).HasColumnName("name_ru");
        builder.Property(warehouse => warehouse.Building).HasColumnName("building");
        builder.Property(warehouse => warehouse.Industrial).HasColumnName("industrial");
        builder.Property(warehouse => warehouse.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(warehouse => warehouse.Geom).HasColumnName("geom");
        builder.Property(warehouse => warehouse.Centroid).HasColumnName("centroid");
    }
}
