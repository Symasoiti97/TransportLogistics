using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Converters;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const string UniqueEmailConstraintName = "IX_users_Email";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(256)
            .HasConversion<EmailConverter>();

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName(UniqueEmailConstraintName);

        builder.Property(user => user.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(user => user.CreatedDate)
            .IsRequired();

        builder.Property(user => user.UpdatedDate)
            .IsRequired();

        builder.Ignore(user => user.Events);
    }
}