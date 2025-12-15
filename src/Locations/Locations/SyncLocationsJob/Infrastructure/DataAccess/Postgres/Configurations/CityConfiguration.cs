using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("cities");
        builder.HasKey(city => city.OsmId);
        builder.Property(city => city.OsmId).HasColumnName("osm_id");
        builder.Property(city => city.OsmType).HasColumnName("osm_type");
        builder.Property(city => city.Name).HasColumnName("name");
        builder.Property(city => city.NameEn).HasColumnName("name_en");
        builder.Property(city => city.NameRu).HasColumnName("name_ru");
        builder.Property(city => city.Place).HasColumnName("place");
        builder.Property(city => city.PostalCode).HasColumnName("postal_code");
        builder.Property(city => city.Population).HasColumnName("population");
        builder.Property(city => city.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(city => city.Geom).HasColumnName("geom");
        builder.Property(city => city.Centroid).HasColumnName("centroid");
    }
}
