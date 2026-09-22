using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDto"/>.
/// Covers intent Presentation-001.
/// </summary>
[TestFixture]
public class CreatePersonDtoTests
{
    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);

    /// <summary>
    /// Presentation-001: Verify DTO can be created with all valid required fields.
    /// </summary>
    [Test]
    [Description("Presentation-001: DTO created with valid required fields sets properties correctly")]
    public void Constructor_ValidRequiredFields_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo(ValidFirstName));
            Assert.That(dto.LastName, Is.EqualTo(ValidLastName));
            Assert.That(dto.Email, Is.EqualTo(ValidEmail));
            Assert.That(dto.IdentityNumber, Is.EqualTo(ValidIdentityNumber));
            Assert.That(dto.BirthDate, Is.EqualTo(ValidBirthDate));
        });
    }
}
