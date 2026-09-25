using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="University"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-003.
/// </summary>
[TestFixture]
public class UniversityTests
{
    // Valid test data constants
    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

    /// <summary>
    /// Domain-001: Verify that a University entity can be created with valid name and country,
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Valid name and country creates University with correct properties")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var university = new University(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(university.Name, Is.EqualTo(ValidName));
            Assert.That(university.Country, Is.EqualTo(ValidCountry));
        });
    }

    /// <summary>
    /// Domain-002 and Domain-003: Verify that creating a University with a null or empty
    /// property throws ArgumentException with the appropriate message for the specific property.
    /// </summary>
    [TestCase("", ValidCountry, "Name",
        Description = "Domain-002: Empty name throws ArgumentException")]
    [TestCase(ValidName, "", "Country",
        Description = "Domain-003: Empty country throws ArgumentException")]
    public void Constructor_NullOrEmptyProperty_ThrowsArgumentException(
        string name, string country, string expectedMessageSubstring)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new University(name, country);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedMessageSubstring));
        });
    }
}
