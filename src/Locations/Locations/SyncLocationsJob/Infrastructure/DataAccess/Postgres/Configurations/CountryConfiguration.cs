using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Models;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.DataAccess.Postgres.Configurations;

internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("countries");
        builder.HasKey(country => country.OsmId);
        builder.Property(country => country.OsmId).HasColumnName("osm_id");
        builder.Property(country => country.Name).HasColumnName("name");
        builder.Property(country => country.NameEn).HasColumnName("name_en");
        builder.Property(country => country.NameRu).HasColumnName("name_ru");
        builder.Property(country => country.Iso3166_1).HasColumnName("iso3166_1");
        builder.Property(country => country.Iso3166_1Alpha2).HasColumnName("iso3166_1_alpha2");
        builder.Property(country => country.Iso3166_1Alpha3).HasColumnName("iso3166_1_alpha3");
        builder.Property(country => country.AdminLevel).HasColumnName("admin_level");
        builder.Property(country => country.Population).HasColumnName("population");
        builder.Property(country => country.Tags).HasColumnName("tags").HasColumnType("jsonb");
        builder.Property(country => country.Geom).HasColumnName("geom");
        builder.Property(country => country.Centroid).HasColumnName("centroid");
    }
}
