using System.Diagnostics.CodeAnalysis;
using UserPhoneNumber = TL.Socials.Identity.Business.Aggregates.UserAggregate.PhoneNumber;

namespace TL.Socials.Identity.Startups.Tests.Aggregates.UserAggregate;

public sealed class PhoneNumberTests
{
    [Theory]
    [InlineData("+14155552671")]
    [InlineData("+79261234567")]
    [InlineData("+442071838750")]
    [InlineData("+33123456789")]
    [InlineData("+4915123456789")]
    public void Parse_AlreadyE164Format_ShouldReturnSameValue(string validE164Number)
    {
        // Act
        var phoneNumber = UserPhoneNumber.Parse(validE164Number, null);
        
        // Assert
        phoneNumber.Value.Should().Be(validE164Number);
    }

    [Theory]
    [InlineData("+14155552671", "+14155552671")]
    [InlineData("+1 415 555 2671", "+14155552671")]
    public void Parse_ValidUSNumbers_ShouldReturnE164Format(string input, string expectedE164)
    {
        // Act
        var phoneNumber = UserPhoneNumber.Parse(input, null);
        
        // Assert
        phoneNumber.Value.Should().Be(expectedE164);
    }

    [Theory]
    [InlineData("+79261234567", "+79261234567")]
    [InlineData("+7 926 123 45 67", "+79261234567")]
    [InlineData("+7 (926) 123-45-67", "+79261234567")]
    public void Parse_ValidRussianNumbers_ShouldReturnE164Format(string input, string expectedE164)
    {
        // Act
        var phoneNumber = UserPhoneNumber.Parse(input, null);
        
        // Assert
        phoneNumber.Value.Should().Be(expectedE164);
    }

    [Theory]
    [InlineData("+442071838750", "+442071838750")]
    [InlineData("+44 20 7183 8750", "+442071838750")]
    public void Parse_ValidUKNumbers_ShouldReturnE164Format(string input, string expectedE164)
    {
        // Act
        var phoneNumber = UserPhoneNumber.Parse(input, null);
        
        // Assert
        phoneNumber.Value.Should().Be(expectedE164);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_NullOrWhiteSpace_ShouldThrowArgumentException(string? invalidInput)
    {
        // Act
        var action = () => UserPhoneNumber.Parse(invalidInput!, null);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("not a phone number")]
    [InlineData("123")]
    [InlineData("+0000000000")]
    public void Parse_InvalidPhoneNumbers_ShouldThrowException(string invalidInput)
    {
        // Act
        var action = () => UserPhoneNumber.Parse(invalidInput, null);
        
        // Assert
        action.Should().Throw<Exception>();
    }

    [Theory]
    [InlineData("+14155552671")]
    [InlineData("+79261234567")]
    [InlineData("+442071838750")]
    public void TryParse_ValidNumbers_ShouldReturnTrue([NotNullWhen(true)] string validInput)
    {
        // Act
        var result = UserPhoneNumber.TryParse(validInput, null, out var phoneNumber);
        
        // Assert
        result.Should().BeTrue();
        phoneNumber.Should().NotBeNull();
        phoneNumber!.Value.Should().StartWith("+");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not a phone number")]
    [InlineData("123")]
    public void TryParse_InvalidNumbers_ShouldReturnFalse(string? invalidInput)
    {
        // Act
        var result = UserPhoneNumber.TryParse(invalidInput, null, out var phoneNumber);
        
        // Assert
        result.Should().BeFalse();
        phoneNumber.Should().BeNull();
    }

    [Fact]
    public void PhoneNumbers_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var value = "+14155552671";
        var phoneNumber1 = UserPhoneNumber.Parse(value, null);
        var phoneNumber2 = UserPhoneNumber.Parse(value, null);
        
        // Assert
        phoneNumber1.Should().Be(phoneNumber2);
        phoneNumber1.GetHashCode().Should().Be(phoneNumber2.GetHashCode());
        (phoneNumber1 == phoneNumber2).Should().BeTrue();
    }

    [Fact]
    public void PhoneNumbers_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var phoneNumber1 = UserPhoneNumber.Parse("+14155552671", null);
        var phoneNumber2 = UserPhoneNumber.Parse("+14155552672", null);
        
        // Assert
        phoneNumber1.Should().NotBe(phoneNumber2);
        phoneNumber1.GetHashCode().Should().NotBe(phoneNumber2.GetHashCode());
        (phoneNumber1 != phoneNumber2).Should().BeTrue();
    }

    [Fact]
    public void PhoneNumber_ParsedFromDifferentFormats_ShouldBeEqual()
    {
        // Arrange
        var phoneNumber1 = UserPhoneNumber.Parse("+14155552671", null);
        var phoneNumber2 = UserPhoneNumber.Parse("+1 415 555 2671", null);
        var phoneNumber3 = UserPhoneNumber.Parse("+1-415-555-2671", null);
        
        // Assert
        phoneNumber1.Should().Be(phoneNumber2);
        phoneNumber2.Should().Be(phoneNumber3);
        phoneNumber1.GetHashCode().Should().Be(phoneNumber2.GetHashCode());
        phoneNumber2.GetHashCode().Should().Be(phoneNumber3.GetHashCode());
    }
}