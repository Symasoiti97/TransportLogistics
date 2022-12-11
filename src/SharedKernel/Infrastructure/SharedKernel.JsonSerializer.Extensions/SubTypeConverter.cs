using System.Text.Json;
using System.Text.Json.Serialization;

namespace TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

public sealed class SubTypeConverter<T> : JsonConverter<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new JsonException($"Type {nameof(T)} not supported for reading");
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            System.Text.Json.JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}