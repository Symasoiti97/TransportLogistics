using System.Text.Json;
using System.Text.Json.Serialization;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis.Converters;

/// <summary>
/// Serializes <see cref="PhoneNumber"/> as a string. The type has a private constructor, so
/// System.Text.Json cannot create it by default.
/// </summary>
public sealed class PhoneNumberJsonConverter : JsonConverter<PhoneNumber>
{
    public override PhoneNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TokenType == JsonTokenType.Null ? null : PhoneNumber.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, PhoneNumber value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.Value);
}
