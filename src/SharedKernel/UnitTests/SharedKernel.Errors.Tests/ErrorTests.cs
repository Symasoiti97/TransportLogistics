using System.Text.Json;
using FluentAssertions;
using TL.SharedKernel.Business.Aggregates;

namespace SharedKernel.Errors.Tests;

public class ErrorTests
{
    [Theory]
    [InlineData((object?) null)]
    public void ThrowErrorIfValueNull_Test2(object? value)
    {
        var action = () => Error.Throw().IfNull(value);

        var errorException = action.Should().Throw<ErrorException>();
        var error = errorException.Which.Error;
        error.Type.Should().Be(JsonNamingPolicy.CamelCase.ConvertName(error.GetType().Name));
    }
}