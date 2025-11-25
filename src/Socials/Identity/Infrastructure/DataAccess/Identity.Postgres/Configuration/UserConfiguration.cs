using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;
using TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Converters;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public const string UniqueEmailConstraintName = "ix_users_email";
    public const string UniquePhoneNumberConstraintName = "ix_users_phone_number";

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedNever();

        builder.Property(user => user.Email)
            .HasMaxLength(256)
            .HasConversion<EmailConverter>();

        builder.HasIndex(user => user.Email)
            .IsUnique()
            .HasDatabaseName(UniqueEmailConstraintName);

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(512);

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(20)
            .HasConversion<PhoneNumberConverter>();

        builder.HasIndex(user => user.PhoneNumber)
            .IsUnique()
            .HasDatabaseName(UniquePhoneNumberConstraintName);

        builder.Property(user => user.CreatedDate)
            .IsRequired();

        builder.Property(user => user.UpdatedDate)
            .IsRequired();

        builder.Ignore(user => user.Events);
    }
}
