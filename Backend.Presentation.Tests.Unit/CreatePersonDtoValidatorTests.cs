using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDtoValidator.Validate"/>.
/// Covers intents Presentation-002 through Presentation-007.
/// </summary>
[TestFixture]
public class CreatePersonDtoValidatorTests
{
    private CreatePersonDtoValidator _sut = null!;

    // Valid test data constants
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        _sut = new CreatePersonDtoValidator();
    }

    /// <summary>
    /// Presentation-002: Verify that validation fails when FirstName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: null FirstName fails validation")]
    [TestCase("", Description = "Presentation-002: empty FirstName fails validation")]
    public void Validate_NullOrEmptyFirstName_ValidationFails(string? firstName)
    {
        // Arrange
        var dto = new CreatePersonDto(firstName!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "FirstName"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that validation fails when LastName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-003: null LastName fails validation")]
    [TestCase("", Description = "Presentation-003: empty LastName fails validation")]
    public void Validate_NullOrEmptyLastName_ValidationFails(string? lastName)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, lastName!, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "LastName"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that validation fails when Email is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-004: null Email fails validation")]
    [TestCase("", Description = "Presentation-004: empty Email fails validation")]
    public void Validate_NullOrEmptyEmail_ValidationFails(string? email)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, email!, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that validation fails when Email format is invalid.
    /// </summary>
    [TestCase("not-an-email", Description = "Presentation-005: Email without @ fails validation")]
    [TestCase("missing@domain", Description = "Presentation-005: Email without TLD fails validation")]
    [TestCase("@nodomain.com", Description = "Presentation-005: Email without local part fails validation")]
    public void Validate_InvalidEmailFormat_ValidationFails(string invalidEmail)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, invalidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-006: Verify that validation fails when IdentityNumber is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-006: null IdentityNumber fails validation")]
    [TestCase("", Description = "Presentation-006: empty IdentityNumber fails validation")]
    public void Validate_NullOrEmptyIdentityNumber_ValidationFails(string? identityNumber)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, identityNumber!, ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "IdentityNumber"));
        });
    }

    /// <summary>
    /// Presentation-007: Verify that validation fails when BirthDate is in the future.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate fails validation")]
    public void Validate_FutureBirthDate_ValidationFails()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddYears(1);
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, futureBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "BirthDate"));
        });
    }
}
