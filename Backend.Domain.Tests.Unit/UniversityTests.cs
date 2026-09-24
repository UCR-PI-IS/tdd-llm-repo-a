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
    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

    /// <summary>
    /// Domain-001: Verify that a University entity can be created with valid name and country
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Valid University creation with name and country sets properties correctly")]
    public void Constructor_ValidNameAndCountry_PropertiesSetCorrectly()
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
    /// Domain-002: Verify that creating a University with null or empty name throws ArgumentException
    /// with the appropriate message.
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-002: Empty name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new University(invalidName!, ValidCountry);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Name"));
        });
    }

    /// <summary>
    /// Domain-003: Verify that creating a University with null or empty country throws ArgumentException
    /// with the appropriate message.
    /// </summary>
    [TestCase(null, Description = "Domain-003: Null country throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty country throws ArgumentException")]
    public void Constructor_NullOrEmptyCountry_ThrowsArgumentException(string? invalidCountry)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new University(ValidName, invalidCountry!);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Country"));
        });
    }
}
