using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

namespace TL.SharedKernel.Errors.Tests;

public sealed partial class JsonSerializingTests
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = {info => ErrorJsonTypeInfoModifier.Modify(info, [typeof(InvalidParameters)])}
        }
    };

    private static object[][] Data() =>
    [
        [
            new InvalidParameters(
            [
                new InvalidParameters.Parameter(null, "name", "$path", "Message", null)
            ]),
            """{"type":"invalid-parameters","parameters":[{"name":"name","path":"$path","message":"Message"}]}"""
        ]
    ];
}