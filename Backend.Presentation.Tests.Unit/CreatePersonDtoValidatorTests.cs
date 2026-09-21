using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDtoValidator"/>.
/// Covers intents Presentation-002 through Presentation-007.
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
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);

    [SetUp]
    public void SetUp()
    {
        _validator = new CreatePersonDtoValidator();
    }

    /// <summary>
    /// Presentation-002: Verify validation fails when FirstName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: Null FirstName fails validation")]
    [TestCase("", Description = "Presentation-002: Empty FirstName fails validation")]
    public void Validate_NullOrEmptyFirstName_ReturnsValidationError(string? firstName)
    {
        // Arrange
        var dto = new CreatePersonDto(firstName!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

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
    public void Validate_NullOrEmptyLastName_ReturnsValidationError(string? lastName)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, lastName!, ValidEmail, ValidIdentityNumber, ValidBirthDate);

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
    public void Validate_NullOrEmptyEmail_ReturnsValidationError(string? email)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, email!, ValidIdentityNumber, ValidBirthDate);

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
    [TestCase("invalid-email", Description = "Presentation-005: Invalid email format fails validation")]
    [TestCase("not-an-email", Description = "Presentation-005: Invalid email format fails validation")]
    [TestCase("@example.com", Description = "Presentation-005: Invalid email format fails validation")]
    public void Validate_InvalidEmailFormat_ReturnsValidationError(string email)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, email, ValidIdentityNumber, ValidBirthDate);

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
    public void Validate_NullOrEmptyIdentityNumber_ReturnsValidationError(string? identityNumber)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, identityNumber!, ValidBirthDate);

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
    public void Validate_FutureBirthDate_ReturnsValidationError()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, futureDate);

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
