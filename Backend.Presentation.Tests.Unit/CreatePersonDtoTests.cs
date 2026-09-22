using NUnit.Framework;
using System;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDto"/>.
/// Covers intent Presentation-001.
/// </summary>
[TestFixture]
public class CreatePersonDtoTests
{
    /// <summary>
    /// Presentation-001: Verify DTO can be created with all valid required fields.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify DTO can be created with all valid required fields")]
    public void Constructor_ValidFields_AllPropertiesSetCorrectly()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 15);

        // Act
        var dto = new CreatePersonDto(firstName, lastName, email, identityNumber, birthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo(firstName));
            Assert.That(dto.LastName, Is.EqualTo(lastName));
            Assert.That(dto.Email, Is.EqualTo(email));
            Assert.That(dto.IdentityNumber, Is.EqualTo(identityNumber));
            Assert.That(dto.BirthDate, Is.EqualTo(birthDate));
        });
    }
}
