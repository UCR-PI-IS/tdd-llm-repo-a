using NUnit.Framework;
using System;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Person"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-017.
/// </summary>
[TestFixture]
public class PersonTests
{
    private static readonly Guid ValidId = Guid.NewGuid();
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 15);
    private const string ValidPhone = "555-1234";

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a Person entity can be created with all valid required fields")]
    public void Constructor_ValidRequiredFields_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(person.Id, Is.EqualTo(ValidId));
            Assert.That(person.FirstName, Is.EqualTo(ValidFirstName));
            Assert.That(person.LastName, Is.EqualTo(ValidLastName));
            Assert.That(person.Email, Is.EqualTo(ValidEmail));
            Assert.That(person.IdentityNumber, Is.EqualTo(ValidIdentityNumber));
            Assert.That(person.BirthDate, Is.EqualTo(ValidBirthDate));
        });
    }

    /// <summary>
    /// Domain-002 through Domain-005, Domain-007 through Domain-010, Domain-013 through Domain-016:
    /// Verify that creating a Person with null, empty, or whitespace-only required fields
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(null, "email", Description = "Domain-002: Null email throws ArgumentException")]
    [TestCase(null, "firstName", Description = "Domain-003: Null firstName throws ArgumentException")]
    [TestCase(null, "lastName", Description = "Domain-004: Null lastName throws ArgumentException")]
    [TestCase(null, "identityNumber", Description = "Domain-005: Null identityNumber throws ArgumentException")]
    [TestCase("", "email", Description = "Domain-007: Empty email throws ArgumentException")]
    [TestCase("", "firstName", Description = "Domain-008: Empty firstName throws ArgumentException")]
    [TestCase("", "lastName", Description = "Domain-009: Empty lastName throws ArgumentException")]
    [TestCase("", "identityNumber", Description = "Domain-010: Empty identityNumber throws ArgumentException")]
    [TestCase("   ", "email", Description = "Domain-013: Whitespace-only email throws ArgumentException")]
    [TestCase("   ", "firstName", Description = "Domain-014: Whitespace-only firstName throws ArgumentException")]
    [TestCase("   ", "lastName", Description = "Domain-015: Whitespace-only lastName throws ArgumentException")]
    [TestCase("   ", "identityNumber", Description = "Domain-016: Whitespace-only identityNumber throws ArgumentException")]
    public void Constructor_InvalidRequiredField_ThrowsArgumentException(string? invalidValue, string expectedParamName)
    {
        // Arrange
        var firstName = expectedParamName == "firstName" ? invalidValue! : ValidFirstName;
        var lastName = expectedParamName == "lastName" ? invalidValue! : ValidLastName;
        var email = expectedParamName == "email" ? invalidValue! : ValidEmail;
        var identityNumber = expectedParamName == "identityNumber" ? invalidValue! : ValidIdentityNumber;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, firstName, lastName, email, identityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(expectedParamName));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Person with a future BirthDate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Future BirthDate throws ArgumentException")]
    public void Constructor_FutureBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var futureBirthDate = DateTime.Now.AddDays(1);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, futureBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("birthDate"));
        });
    }

    /// <summary>
    /// Domain-017: Verify that creating a Person with today's date as BirthDate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-017: Today as BirthDate throws ArgumentException")]
    public void Constructor_TodayBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var today = DateTime.Today;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, today);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("birthDate"));
        });
    }

    /// <summary>
    /// Domain-011: Verify that a Person can be created with optional Phone field provided.
    /// </summary>
    [Test]
    [Description("Domain-011: Person created with optional Phone field provided")]
    public void Constructor_WithPhone_PhonePropertySet()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, ValidPhone);

        // Assert
        Assert.That(person.Phone, Is.EqualTo(ValidPhone));
    }

    /// <summary>
    /// Domain-012: Verify that a Person can be created with null Phone (optional field).
    /// </summary>
    [Test]
    [Description("Domain-012: Person created with null Phone (optional field)")]
    public void Constructor_WithNullPhone_PhonePropertyIsNull()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }
}
