using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Postgres.Converters;

internal sealed class PhoneNumberConverter() : ValueConverter<PhoneNumber?, string?>(
    phoneNumber => phoneNumber != null ? phoneNumber.Value : null,
    value => value != null ? new PhoneNumber(value) : null);