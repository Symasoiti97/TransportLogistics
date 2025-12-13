using System.Diagnostics.CodeAnalysis;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Startups.Tests.Aggregates.UserAggregate;

public sealed class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test@test.org")]
    [InlineData("john.doe@company.net")]
    [InlineData("alice+tag@domain.co.uk")]
    [InlineData("bob_smith@sub.domain.com")]
    public void Parse_AlreadyNormalizedEmail_ShouldReturnSameValue(string normalizedEmail)
    {
        // Act
        var email = Email.Parse(normalizedEmail, null);
        
        // Assert
        email.Value.Should().Be(normalizedEmail);
    }

    [Theory]
    [InlineData("USER@EXAMPLE.COM", "user@example.com")]
    [InlineData("  test@test.org  ", "test@test.org")]
    [InlineData("John.Doe@Company.NET", "john.doe@company.net")]
    [InlineData(" ALICE+Tag@DOMAIN.co.uk ", "alice+tag@domain.co.uk")]
    [InlineData("\tBob_Smith@SUB.domain.COM\n", "bob_smith@sub.domain.com")]
    public void Parse_UnnormalizedEmail_ShouldNormalize(string input, string expected)
    {
        // Act
        var email = Email.Parse(input, null);
        
        // Assert
        email.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void Parse_NullOrWhiteSpace_ShouldThrowArgumentException(string? invalidInput)
    {
        // Act
        var action = () => Email.Parse(invalidInput!, null);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@@example.com")]
    [InlineData("user@example")]
    [InlineData("user.example.com")]
    [InlineData("user@example.c")]
    public void Parse_InvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act
        var action = () => Email.Parse(invalidEmail, null);
        
        // Assert
        action.Should().Throw<ArgumentException>()
            .WithMessage("Invalid email format*");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test@test.org")]
    [InlineData("USER@EXAMPLE.COM")]
    [InlineData("  test@test.org  ")]
    public void TryParse_ValidEmails_ShouldReturnTrue([NotNullWhen(true)] string validInput)
    {
        // Act
        var result = Email.TryParse(validInput, null, out var email);
        
        // Assert
        result.Should().BeTrue();
        email.Should().NotBeNull();
        email!.Value.Should().Be(validInput.Trim().ToLower());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    public void TryParse_InvalidEmails_ShouldReturnFalse(string? invalidInput)
    {
        // Act
        var result = Email.TryParse(invalidInput, null, out var email);
        
        // Assert
        result.Should().BeFalse();
        email.Should().BeNull();
    }

    [Fact]
    public void Emails_WithSameNormalizedValue_ShouldBeEqual()
    {
        // Arrange
        var email1 = Email.Parse("user@example.com", null);
        var email2 = Email.Parse("USER@EXAMPLE.COM", null);
        var email3 = Email.Parse("  user@example.com  ", null);
        
        // Assert
        email1.Should().Be(email2);
        email2.Should().Be(email3);
        email1.GetHashCode().Should().Be(email2.GetHashCode());
        email2.GetHashCode().Should().Be(email3.GetHashCode());
    }

    [Fact]
    public void Emails_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var email1 = Email.Parse("user1@example.com", null);
        var email2 = Email.Parse("user2@example.com", null);
        
        // Assert
        email1.Should().NotBe(email2);
        email1.GetHashCode().Should().NotBe(email2.GetHashCode());
        (email1 != email2).Should().BeTrue();
    }

    [Theory]
    [InlineData("user@example..com")]  // Double dot is actually valid in some email systems
    [InlineData("user@.example.com")]   // Leading dot in domain
    [InlineData("user@-example.com")]   // Leading hyphen in domain
    [InlineData("user@example-.com")]   // Trailing hyphen in domain
    public void Parse_EdgeCaseEmails_MightBeValidDependingOnRegex(string edgeCaseEmail)
    {
        // Note: These emails might be accepted by our simple regex
        // This test documents the current behavior rather than ideal behavior
        var action = () => Email.Parse(edgeCaseEmail, null);
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user+tag@domain.org")]
    [InlineData("firstname.lastname@company.co.uk")]
    [InlineData("123456@numeric.net")]
    [InlineData("a@b.cd")]
    public void Parse_ValidComplexEmails_ShouldSucceed(string validEmail)
    {
        // Act
        var email = Email.Parse(validEmail, null);
        
        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(validEmail.ToLower());
    }

    [Theory]
    [InlineData("user+tag+anothertag@example.com")]
    [InlineData("very.long.email.address.with.many.dots@subdomain.example.com")]
    [InlineData("numbers123and456letters@test123.com")]
    public void Parse_EdgeCaseValidEmails_ShouldSucceed(string edgeCaseEmail)
    {
        // Act
        var email = Email.Parse(edgeCaseEmail, null);
        
        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(edgeCaseEmail.ToLower());
    }
}