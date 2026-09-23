using FluentValidation;
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
    private CreatePersonDtoValidator _sut = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@email.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        _sut = new CreatePersonDtoValidator();
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
    /// Presentation-002 through Presentation-004 and Presentation-006: Verify that validation fails
    /// when a required field is null or empty, with the correct property name in the error.
    /// </summary>
    [TestCase(null, ValidLastName, ValidEmail, ValidIdentityNumber, "FirstName",
        Description = "Presentation-002: FirstName null fails validation")]
    [TestCase("", ValidLastName, ValidEmail, ValidIdentityNumber, "FirstName",
        Description = "Presentation-002: FirstName empty fails validation")]
    [TestCase(ValidFirstName, null, ValidEmail, ValidIdentityNumber, "LastName",
        Description = "Presentation-003: LastName null fails validation")]
    [TestCase(ValidFirstName, "", ValidEmail, ValidIdentityNumber, "LastName",
        Description = "Presentation-003: LastName empty fails validation")]
    [TestCase(ValidFirstName, ValidLastName, null, ValidIdentityNumber, "Email",
        Description = "Presentation-004: Email null fails validation")]
    [TestCase(ValidFirstName, ValidLastName, "", ValidIdentityNumber, "Email",
        Description = "Presentation-004: Email empty fails validation")]
    [TestCase(ValidFirstName, ValidLastName, ValidEmail, null, "IdentityNumber",
        Description = "Presentation-006: IdentityNumber null fails validation")]
    [TestCase(ValidFirstName, ValidLastName, ValidEmail, "", "IdentityNumber",
        Description = "Presentation-006: IdentityNumber empty fails validation")]
    public void Validate_NullOrEmptyRequiredField_ReturnsInvalid(
        string? firstName, string? lastName, string? email, string? identityNumber,
        string expectedProperty)
    {
        // Arrange
        var dto = new CreatePersonDto(
            firstName!,
            lastName!,
            email!,
            identityNumber!,
            ValidBirthDate);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == expectedProperty));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that validation fails when the Email format is invalid.
    /// </summary>
    [TestCase("not-an-email", Description = "Presentation-005: Invalid email format (no @)")]
    [TestCase("missing@domain", Description = "Presentation-005: Invalid email format (no TLD)")]
    [TestCase("@nodomain.com", Description = "Presentation-005: Invalid email format (no local part)")]
    public void Validate_InvalidEmailFormat_ReturnsInvalid(string invalidEmail)
    {
        // Arrange
        var dto = CreateValidDto(email: invalidEmail);

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
    /// Presentation-007: Verify that validation fails when BirthDate is in the future.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate fails validation")]
    public void Validate_FutureBirthDate_ReturnsInvalid()
    {
        // Arrange
        var futureBirthDate = DateTime.Now.AddDays(30);
        var dto = CreateValidDto(birthDate: futureBirthDate);

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
