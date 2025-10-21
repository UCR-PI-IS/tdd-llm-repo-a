using FluentAssertions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.ValueObject.AccountManagement;

public class UserNameTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void TryCreate_WithNullOrWhitespace_ReturnsFalse(string? input)
    {
        var result = UserName.TryCreate(input, out var userName);

        result.Should().BeFalse();
        userName.Should().BeNull();
    }

    [Theory]
    [InlineData("UserName")]
    [InlineData("user name")]
    [InlineData("user$name")]
    [InlineData("user-name")]
    [InlineData("123456789012345678901234567890123456789012345678901")]
    public void TryCreate_WithInvalidFormat_ReturnsFalse(string input)
    {
        var result = UserName.TryCreate(input, out var userName);

        result.Should().BeFalse();
        userName.Should().BeNull();
    }

    [Theory]
    [InlineData("username")]
    [InlineData("user.name")]
    [InlineData("user_name")]
    [InlineData("abc123")]
    [InlineData("a")]
    [InlineData("user1234567890123456789012345678901234567890")]
    public void TryCreate_WithValidFormat_ReturnsTrue(string input)
    {
        var result = UserName.TryCreate(input, out var userName);

        result.Should().BeTrue();
        userName.Should().NotBeNull();
        userName!.Value.Should().Be(input);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("User Name")]
    public void Create_WithInvalidInput_ThrowsValidationException(string? input)
    {
        Action act = () => UserName.Create(input);

        act.Should()
           .ThrowExactly<ValidationException>()
           .WithMessage("*Invalid UserName format*");
    }

    [Theory]
    [InlineData("valid.username")]
    [InlineData("valid_user")]
    public void Create_WithValidInput_ReturnsUserNameInstance(string input)
    {
        var result = UserName.Create(input);

        result.Should().NotBeNull();
        result.Value.Should().Be(input);
    }

    [Fact]
    public void UserNames_WithSameValue_AreEqual()
    {
        var input = "user123";
        var u1 = UserName.Create(input);
        var u2 = UserName.Create(input);

        u1.Should().Be(u2);
    }

    [Fact]
    public void UserNames_WithDifferentValues_AreNotEqual()
    {
        var u1 = UserName.Create("user1");
        var u2 = UserName.Create("user2");

        u1.Should().NotBe(u2);
    }

}
