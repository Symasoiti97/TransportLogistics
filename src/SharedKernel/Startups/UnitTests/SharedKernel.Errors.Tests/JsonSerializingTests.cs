using System.Text.Json;
using FluentAssertions;
using TL.SharedKernel.Business.Aggregates;

namespace TL.SharedKernel.Errors.Tests;

public sealed partial class JsonSerializingTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void Serialize_Test(Error error, string expectedJson)
    {
        var actualJson = JsonSerializer.Serialize(error, JsonSerializerOptions);
        actualJson.Should().Be(expectedJson);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Deserialize_Test(Error expectedError, string actualJson)
    {
        var actualError = JsonSerializer.Deserialize<Error>(actualJson, JsonSerializerOptions);
        actualError.Should().BeEquivalentTo(expectedError);
    }
}