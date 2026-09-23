using FluentValidation.Results;
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
    private static readonly DateTime ValidBirthDate = new(1990, 5, 15);

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
    /// The validator should produce a validation error for the FirstName property.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: null FirstName fails validation")]
    [TestCase("", Description = "Presentation-002: empty FirstName fails validation")]
    public void Validate_NullOrEmptyFirstName_HasValidationError(string? firstName)
    {
        // Arrange
        var dto = CreateValidDto(firstName: firstName!);

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
    /// The validator should produce a validation error for the LastName property.
    /// </summary>
    [TestCase(null, Description = "Presentation-003: null LastName fails validation")]
    [TestCase("", Description = "Presentation-003: empty LastName fails validation")]
    public void Validate_NullOrEmptyLastName_HasValidationError(string? lastName)
    {
        // Arrange
        var dto = CreateValidDto(lastName: lastName!);

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
    /// The validator should produce a validation error for the Email property.
    /// </summary>
    [TestCase(null, Description = "Presentation-004: null Email fails validation")]
    [TestCase("", Description = "Presentation-004: empty Email fails validation")]
    public void Validate_NullOrEmptyEmail_HasValidationError(string? email)
    {
        // Arrange
        var dto = CreateValidDto(email: email!);

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
    /// The validator should produce a validation error for the Email property.
    /// </summary>
    [TestCase("not-an-email", Description = "Presentation-005: invalid email format (no @) fails validation")]
    [TestCase("missing@domain", Description = "Presentation-005: invalid email format (no TLD) fails validation")]
    [TestCase("@nodomain.com", Description = "Presentation-005: invalid email format (no local part) fails validation")]
    public void Validate_InvalidEmailFormat_HasValidationError(string invalidEmail)
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
    /// The validator should produce a validation error for the IdentityNumber property.
    /// </summary>
    [TestCase(null, Description = "Presentation-006: null IdentityNumber fails validation")]
    [TestCase("", Description = "Presentation-006: empty IdentityNumber fails validation")]
    public void Validate_NullOrEmptyIdentityNumber_HasValidationError(string? identityNumber)
    {
        // Arrange
        var dto = CreateValidDto(identityNumber: identityNumber!);

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
    /// The validator should produce a validation error for the BirthDate property.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate fails validation")]
    public void Validate_FutureBirthDate_HasValidationError()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddDays(30);
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
