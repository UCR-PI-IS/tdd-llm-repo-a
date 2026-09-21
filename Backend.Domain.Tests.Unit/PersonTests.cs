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
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);
    private const string ValidPhone = "+1234567890";

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
    /// Domain-002: Verify that creating a Person with null email throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Null email throws ArgumentException")]
    public void Constructor_NullEmail_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, null!, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("email"));
        });
    }

    /// <summary>
    /// Domain-003: Verify that creating a Person with null firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Null firstName throws ArgumentException")]
    public void Constructor_NullFirstName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, null!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("firstName"));
        });
    }

    /// <summary>
    /// Domain-004: Verify that creating a Person with null lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Null lastName throws ArgumentException")]
    public void Constructor_NullLastName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, null!, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("lastName"));
        });
    }

    /// <summary>
    /// Domain-005: Verify that creating a Person with null identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Null identityNumber throws ArgumentException")]
    public void Constructor_NullIdentityNumber_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, null!, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("identityNumber"));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Person with future BirthDate throws ArgumentException.
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
    /// Domain-007: Verify that creating a Person with empty email throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Empty email throws ArgumentException")]
    public void Constructor_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, "", ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("email"));
        });
    }

    /// <summary>
    /// Domain-008: Verify that creating a Person with empty firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Empty firstName throws ArgumentException")]
    public void Constructor_EmptyFirstName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, "", ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("firstName"));
        });
    }

    /// <summary>
    /// Domain-009: Verify that creating a Person with empty lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-009: Empty lastName throws ArgumentException")]
    public void Constructor_EmptyLastName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, "", ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("lastName"));
        });
    }

    /// <summary>
    /// Domain-010: Verify that creating a Person with empty identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-010: Empty identityNumber throws ArgumentException")]
    public void Constructor_EmptyIdentityNumber_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, "", ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("identityNumber"));
        });
    }

    /// <summary>
    /// Domain-011: Verify that a Person can be created with optional Phone field provided.
    /// </summary>
    [Test]
    [Description("Domain-011: Valid Person creation with optional Phone field")]
    public void Constructor_ValidPhoneProvided_PhonePropertySetCorrectly()
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
    [Description("Domain-012: Valid Person creation with null Phone (optional)")]
    public void Constructor_NullPhone_PhonePropertyIsNull()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }

    /// <summary>
    /// Domain-013: Verify that creating a Person with whitespace-only email throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-013: Whitespace-only email throws ArgumentException")]
    public void Constructor_WhitespaceEmail_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, "   ", ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("email"));
        });
    }

    /// <summary>
    /// Domain-014: Verify that creating a Person with whitespace-only firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-014: Whitespace-only firstName throws ArgumentException")]
    public void Constructor_WhitespaceFirstName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, "   ", ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("firstName"));
        });
    }

    /// <summary>
    /// Domain-015: Verify that creating a Person with whitespace-only lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-015: Whitespace-only lastName throws ArgumentException")]
    public void Constructor_WhitespaceLastName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, "   ", ValidEmail, ValidIdentityNumber, ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("lastName"));
        });
    }

    /// <summary>
    /// Domain-016: Verify that creating a Person with whitespace-only identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-016: Whitespace-only identityNumber throws ArgumentException")]
    public void Constructor_WhitespaceIdentityNumber_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, "   ", ValidBirthDate);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("identityNumber"));
        });
    }

    /// <summary>
    /// Domain-017: Verify that creating a Person with today's date as BirthDate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-017: Today's date as BirthDate throws ArgumentException")]
    public void Constructor_TodayAsBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var today = DateTime.Now.Date;

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
}
