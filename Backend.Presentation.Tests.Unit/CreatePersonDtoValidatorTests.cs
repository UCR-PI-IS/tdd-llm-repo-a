using NUnit.Framework;
using System;
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
    private CreatePersonDtoValidator _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreatePersonDtoValidator();
    }

    private static CreatePersonDto CreateValidDto()
    {
        return new CreatePersonDto("John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 1, 15));
    }

    /// <summary>
    /// Presentation-002: Verify validation fails when FirstName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: Validation fails when FirstName is null")]
    [TestCase("", Description = "Presentation-002: Validation fails when FirstName is empty")]
    public void Validate_InvalidFirstName_ReturnsValidationError(string? firstName)
    {
        // Arrange
        var dto = new CreatePersonDto(firstName!, "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 1, 15));

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Presentation-003: Verify validation fails when LastName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-003: Validation fails when LastName is null")]
    [TestCase("", Description = "Presentation-003: Validation fails when LastName is empty")]
    public void Validate_InvalidLastName_ReturnsValidationError(string? lastName)
    {
        // Arrange
        var dto = new CreatePersonDto("John", lastName!, "john.doe@example.com", "123456789", new DateTime(1990, 1, 15));

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Presentation-004: Verify validation fails when Email is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-004: Validation fails when Email is null")]
    [TestCase("", Description = "Presentation-004: Validation fails when Email is empty")]
    public void Validate_InvalidEmail_ReturnsValidationError(string? email)
    {
        // Arrange
        var dto = new CreatePersonDto("John", "Doe", email!, "123456789", new DateTime(1990, 1, 15));

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Presentation-005: Verify validation fails when Email format is invalid.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify validation fails when Email format is invalid")]
    public void Validate_InvalidEmailFormat_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreatePersonDto("John", "Doe", "invalid-email", "123456789", new DateTime(1990, 1, 15));

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Presentation-006: Verify validation fails when IdentityNumber is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-006: Validation fails when IdentityNumber is null")]
    [TestCase("", Description = "Presentation-006: Validation fails when IdentityNumber is empty")]
    public void Validate_InvalidIdentityNumber_ReturnsValidationError(string? identityNumber)
    {
        // Arrange
        var dto = new CreatePersonDto("John", "Doe", "john.doe@example.com", identityNumber!, new DateTime(1990, 1, 15));

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Presentation-007: Verify validation fails when BirthDate is in the future.
    /// </summary>
    [Test]
    [Description("Presentation-007: Verify validation fails when BirthDate is in the future")]
    public void Validate_FutureBirthDate_ReturnsValidationError()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var dto = new CreatePersonDto("John", "Doe", "john.doe@example.com", "123456789", futureDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Is.Not.Empty);
        });
    }
}
