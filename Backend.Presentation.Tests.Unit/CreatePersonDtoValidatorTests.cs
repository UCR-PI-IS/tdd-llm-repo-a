using FluentValidation.Results;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDtoValidator"/>.
/// Covers intents Presentation-002 through Presentation-007 for SPT-UM-001-003.
/// </summary>
[TestFixture]
public class CreatePersonDtoValidatorTests
{
    private CreatePersonDtoValidator _validator = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        _validator = new CreatePersonDtoValidator();
    }

    private static CreatePersonDto CreateValidDto(
        string firstName = ValidFirstName,
        string lastName = ValidLastName,
        string email = ValidEmail,
        string identityNumber = ValidIdentityNumber,
        DateTime? birthDate = null)
    {
        return new CreatePersonDto(
            firstName,
            lastName,
            email,
            identityNumber,
            birthDate ?? ValidBirthDate);
    }

    /// <summary>
    /// Presentation-002: Verify validation fails when FirstName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: Null FirstName fails validation")]
    [TestCase("", Description = "Presentation-002: Empty FirstName fails validation")]
    public void Validate_NullOrEmptyFirstName_IsInvalid(string? invalidFirstName)
    {
        // Arrange
        var dto = CreateValidDto(firstName: invalidFirstName!);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "FirstName"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify validation fails when LastName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-003: Null LastName fails validation")]
    [TestCase("", Description = "Presentation-003: Empty LastName fails validation")]
    public void Validate_NullOrEmptyLastName_IsInvalid(string? invalidLastName)
    {
        // Arrange
        var dto = CreateValidDto(lastName: invalidLastName!);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "LastName"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify validation fails when Email is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-004: Null Email fails validation")]
    [TestCase("", Description = "Presentation-004: Empty Email fails validation")]
    public void Validate_NullOrEmptyEmail_IsInvalid(string? invalidEmail)
    {
        // Arrange
        var dto = CreateValidDto(email: invalidEmail!);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify validation fails when Email format is invalid.
    /// </summary>
    [TestCase("not-an-email", Description = "Presentation-005: Invalid email format (no @) fails validation")]
    [TestCase("missing@domain", Description = "Presentation-005: Invalid email format (no TLD) fails validation")]
    [TestCase("@nodomain.com", Description = "Presentation-005: Invalid email format (no local part) fails validation")]
    public void Validate_InvalidEmailFormat_IsInvalid(string invalidEmail)
    {
        // Arrange
        var dto = CreateValidDto(email: invalidEmail);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-006: Verify validation fails when IdentityNumber is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-006: Null IdentityNumber fails validation")]
    [TestCase("", Description = "Presentation-006: Empty IdentityNumber fails validation")]
    public void Validate_NullOrEmptyIdentityNumber_IsInvalid(string? invalidIdentityNumber)
    {
        // Arrange
        var dto = CreateValidDto(identityNumber: invalidIdentityNumber!);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "IdentityNumber"));
        });
    }

    /// <summary>
    /// Presentation-007: Verify validation fails when BirthDate is in the future.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate fails validation")]
    public void Validate_FutureBirthDate_IsInvalid()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddDays(1);
        var dto = CreateValidDto(birthDate: futureBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "BirthDate"));
        });
    }
}
