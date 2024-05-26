using FluentAssertions;
using TL.SharedKernel.Business.Aggregates;
using TL.SharedKernel.Infrastructure.JsonSerializer.Extensions;

namespace TL.SharedKernel.Errors.Tests;

public sealed class ErrorTests
{
    [Theory]
    [InlineData((object?) null)]
    public void ThrowErrorIfValueNull_Test2(object? value)
    {
        var action = () => Error.Throw().IfNull(value);

        var errorException = action.Should().Throw<ErrorException>();
        var error = errorException.Which.Error;
        error.BuildType().Should().Be("invalid_value");
    }
}