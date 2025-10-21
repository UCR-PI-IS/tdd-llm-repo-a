using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using FluentAssertions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.ValueObject.AccountManagement;

public class EmailTests
{

    // TryCreate method tests

    [Fact]
    public void TryCreate_WithValidEmail_ShouldReturnTrue()
    {
        // Arrange
        var input = "example@test.com";

        // Act
        var result = Email.TryCreate(input, out var email);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("name.surname@company.org")]
    [InlineData("a@b.cd")]
    public void TryCreate_WithValidEmails_ShouldReturnTrue(string validEmail)
    {
        // Act
        var result = Email.TryCreate(validEmail, out var email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void TryCreate_EmailWithSpecialCharacters_ShouldReturnTrue()
    {
        // Arrange
        var input = "first.last+custom-tag_123@sub.domain.com";

        // Act
        var result = Email.TryCreate(input, out var email);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("missingatsign.com")]
    [InlineData("no.domain@")]
    [InlineData("@nodomain.com")]
    [InlineData("user@.com")]
    [InlineData("user@@domain.com")]
    [InlineData("user@domain,com")]
    [InlineData("user@domain com")]
    [InlineData("user@domaincom")]
    public void TryCreate_InvalidEmails_ShouldReturnFalse(string invalidEmail)
    {
        // Act
        var result = Email.TryCreate(invalidEmail, out var email);

        // Assert
        result.Should().BeFalse(because: "validation should fail when given a invalid email");
    }

    [Fact]
    public void TryCreate_WithEmptyEmail_ShouldReturnFalse()
    {
        // Arrange
        var input = "";

        // Act
        var result = Email.TryCreate(input, out var email);

        // Assert
        result.Should().BeFalse(because: "validation should fail when given a empty email");
    }

    // Create method tests

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("name.surname@company.org")]
    [InlineData("valid.email+tag@sub.domain.net")]
    public void Create_WithValidEmail_ShouldReturnEmailObject(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        email.Should().NotBeNull();
        email.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("noatsign.com")]
    [InlineData("@no-user.com")]
    [InlineData("user@.nodomain")]
    [InlineData("user@@domain.com")]
    [InlineData("user@domain")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowValidationException(string? invalidEmail)
    {
        // Act
        Action act = () => Email.Create(invalidEmail);

        // Assert
        act.Should()
           .Throw<ValidationException>()
           .WithMessage("Invalid email format. It must contain '@' and end with .domain");
    }


    [Fact]
    public void Create_EmailWithWhitespaceAround_ShouldThrow()
    {
        // Arrange
        var input = "  user@example.com ";

        // Act
        Action act = () => Email.Create(input);

        // Assert
        act.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Create_ValidEmail_ShouldProduceEqualObjects()
    {
        // Arrange
        var email1 = Email.Create("same@domain.com");
        var email2 = Email.Create("same@domain.com");

        // Assert
        email1.Should().Be(email2);
    }

    [Fact]
    public void Create_DifferentEmails_ShouldProduceNonEqualObjects()
    {
        // Arrange
        var email1 = Email.Create("user1@domain.com");
        var email2 = Email.Create("user2@domain.com");

        // Assert
        email1.Should().NotBe(email2);
    }

}