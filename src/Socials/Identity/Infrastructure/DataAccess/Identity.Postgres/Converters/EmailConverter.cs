using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Converters;

internal sealed class EmailConverter() : ValueConverter<Email?, string?>(
    email => email != null ? email.Value : null,
    value => value != null ? new Email(value) : null);