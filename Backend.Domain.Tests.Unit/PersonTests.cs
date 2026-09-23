using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Person"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-017.
/// </summary>
[TestFixture]
public class PersonTests
{
    // Valid test data constants
    private const int ValidId = 1;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new(1990, 5, 15);
    private const string ValidPhone = "555-0123";

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Valid Person creation with all required fields sets all properties correctly")]
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
    /// Domain-002 through Domain-005: Verify that creating a Person with a null required string field
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(ValidFirstName, ValidLastName, null, ValidIdentityNumber, "email",
        Description = "Domain-002: null email throws ArgumentException")]
    [TestCase(null, ValidLastName, ValidEmail, ValidIdentityNumber, "firstName",
        Description = "Domain-003: null firstName throws ArgumentException")]
    [TestCase(ValidFirstName, null, ValidEmail, ValidIdentityNumber, "lastName",
        Description = "Domain-004: null lastName throws ArgumentException")]
    [TestCase(ValidFirstName, ValidLastName, ValidEmail, null, "identityNumber",
        Description = "Domain-005: null identityNumber throws ArgumentException")]
    public void Constructor_NullRequiredField_ThrowsArgumentException(
        string? firstName, string? lastName, string? email, string? identityNumber, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, firstName!, lastName!, email!, identityNumber!, ValidBirthDate);
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
    /// Domain-007 through Domain-010: Verify that creating a Person with an empty required string field
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(ValidFirstName, ValidLastName, "", ValidIdentityNumber, "email",
        Description = "Domain-007: empty email throws ArgumentException")]
    [TestCase("", ValidLastName, ValidEmail, ValidIdentityNumber, "firstName",
        Description = "Domain-008: empty firstName throws ArgumentException")]
    [TestCase(ValidFirstName, "", ValidEmail, ValidIdentityNumber, "lastName",
        Description = "Domain-009: empty lastName throws ArgumentException")]
    [TestCase(ValidFirstName, ValidLastName, ValidEmail, "", "identityNumber",
        Description = "Domain-010: empty identityNumber throws ArgumentException")]
    public void Constructor_EmptyRequiredField_ThrowsArgumentException(
        string firstName, string lastName, string email, string identityNumber, string expectedParamName)
    {
        // Arrange & Act
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
    /// Domain-013 through Domain-016: Verify that creating a Person with a whitespace-only required string field
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(ValidFirstName, ValidLastName, "   ", ValidIdentityNumber, "email",
        Description = "Domain-013: whitespace-only email throws ArgumentException")]
    [TestCase("   ", ValidLastName, ValidEmail, ValidIdentityNumber, "firstName",
        Description = "Domain-014: whitespace-only firstName throws ArgumentException")]
    [TestCase(ValidFirstName, "   ", ValidEmail, ValidIdentityNumber, "lastName",
        Description = "Domain-015: whitespace-only lastName throws ArgumentException")]
    [TestCase(ValidFirstName, ValidLastName, ValidEmail, "   ", "identityNumber",
        Description = "Domain-016: whitespace-only identityNumber throws ArgumentException")]
    public void Constructor_WhitespaceOnlyRequiredField_ThrowsArgumentException(
        string firstName, string lastName, string email, string identityNumber, string expectedParamName)
    {
        // Arrange & Act
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
    /// Domain-006: Verify that creating a Person with a future BirthDate
    /// throws ArgumentException with paramName "birthDate".
    /// </summary>
    [Test]
    [Description("Domain-006: Future BirthDate throws ArgumentException")]
    public void Constructor_FutureBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddYears(1);

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
    /// Domain-017: Verify that creating a Person with today's date as BirthDate
    /// throws ArgumentException (must be a past date, not today).
    /// </summary>
    [Test]
    [Description("Domain-017: Today's date as BirthDate throws ArgumentException")]
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
    /// Domain-011 and Domain-012: Verify that a Person can be created with the optional Phone field,
    /// either with a valid value or null.
    /// </summary>
    [TestCase(ValidPhone, Description = "Domain-011: Person created with optional Phone field sets Phone correctly")]
    [TestCase(null, Description = "Domain-012: Person created with null Phone (optional field) sets Phone to null")]
    public void Constructor_OptionalPhoneField_PhonePropertySetCorrectly(string? phone)
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, phone);

        // Assert
        Assert.That(person.Phone, Is.EqualTo(phone));
    }
}
