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
    private const string ValidPhone = "+506-8888-9999";

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Valid Person creation with all required fields")]
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
    /// Domain-011: Verify that a Person can be created with the optional Phone field provided
    /// and the Phone property is correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-011: Valid Person creation with optional Phone field")]
    public void Constructor_WithOptionalPhone_PhonePropertySet()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, ValidPhone);

        // Assert
        Assert.That(person.Phone, Is.EqualTo(ValidPhone));
    }

    /// <summary>
    /// Domain-012: Verify that a Person can be created with null Phone (optional field)
    /// and the Phone property is null.
    /// </summary>
    [Test]
    [Description("Domain-012: Valid Person creation with null Phone (optional field)")]
    public void Constructor_WithNullPhone_PhoneIsNull()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }

    /// <summary>
    /// Domain-002 through Domain-005: Verify that creating a Person with a null required string field
    /// throws ArgumentException with the correct ParamName.
    /// </summary>
    [TestCase("firstName", Description = "Domain-003: null firstName throws ArgumentException")]
    [TestCase("lastName", Description = "Domain-004: null lastName throws ArgumentException")]
    [TestCase("email", Description = "Domain-002: null email throws ArgumentException")]
    [TestCase("identityNumber", Description = "Domain-005: null identityNumber throws ArgumentException")]
    public void Constructor_NullRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            CreatePersonWithNullField(fieldName);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(fieldName));
        });
    }

    /// <summary>
    /// Domain-007 through Domain-010: Verify that creating a Person with an empty required string field
    /// throws ArgumentException with the correct ParamName.
    /// </summary>
    [TestCase("firstName", Description = "Domain-008: empty firstName throws ArgumentException")]
    [TestCase("lastName", Description = "Domain-009: empty lastName throws ArgumentException")]
    [TestCase("email", Description = "Domain-007: empty email throws ArgumentException")]
    [TestCase("identityNumber", Description = "Domain-010: empty identityNumber throws ArgumentException")]
    public void Constructor_EmptyRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            CreatePersonWithField(fieldName, string.Empty);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(fieldName));
        });
    }

    /// <summary>
    /// Domain-013 through Domain-016: Verify that creating a Person with a whitespace-only required string field
    /// throws ArgumentException with the correct ParamName.
    /// </summary>
    [TestCase("firstName", Description = "Domain-014: whitespace-only firstName throws ArgumentException")]
    [TestCase("lastName", Description = "Domain-015: whitespace-only lastName throws ArgumentException")]
    [TestCase("email", Description = "Domain-013: whitespace-only email throws ArgumentException")]
    [TestCase("identityNumber", Description = "Domain-016: whitespace-only identityNumber throws ArgumentException")]
    public void Constructor_WhitespaceRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            CreatePersonWithField(fieldName, "   ");
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(fieldName));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Person with a future BirthDate
    /// throws ArgumentException with ParamName "birthDate".
    /// </summary>
    [Test]
    [Description("Domain-006: Future BirthDate throws ArgumentException")]
    public void Constructor_FutureBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var futureBirthDate = DateTime.Today.AddDays(1);

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

    #region Helper Methods

    /// <summary>
    /// Creates a Person with the specified field set to null, keeping all other fields valid.
    /// </summary>
    private static void CreatePersonWithNullField(string fieldName)
    {
        switch (fieldName)
        {
            case "firstName":
                new Person(ValidId, null!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
                break;
            case "lastName":
                new Person(ValidId, ValidFirstName, null!, ValidEmail, ValidIdentityNumber, ValidBirthDate);
                break;
            case "email":
                new Person(ValidId, ValidFirstName, ValidLastName, null!, ValidIdentityNumber, ValidBirthDate);
                break;
            case "identityNumber":
                new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, null!, ValidBirthDate);
                break;
        }
    }

    /// <summary>
    /// Creates a Person with the specified field set to the given value, keeping all other fields valid.
    /// </summary>
    private static void CreatePersonWithField(string fieldName, string value)
    {
        switch (fieldName)
        {
            case "firstName":
                new Person(ValidId, value, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
                break;
            case "lastName":
                new Person(ValidId, ValidFirstName, value, ValidEmail, ValidIdentityNumber, ValidBirthDate);
                break;
            case "email":
                new Person(ValidId, ValidFirstName, ValidLastName, value, ValidIdentityNumber, ValidBirthDate);
                break;
            case "identityNumber":
                new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, value, ValidBirthDate);
                break;
        }
    }

    #endregion
}
