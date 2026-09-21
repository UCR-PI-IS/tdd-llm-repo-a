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
    private const string ValidId = "PER-001";
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private const string ValidPhone = "555-1234";

    [SetUp]
    public void SetUp()
    {
        // No shared setup needed
    }

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields.
    /// </summary>
    [Test]
    [Description("Domain-001: Person created with all valid required fields has correct property values")]
    public void Constructor_AllValidRequiredFields_AllPropertiesSetCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);

        // Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(person.Id, Is.EqualTo(ValidId));
            Assert.That(person.FirstName, Is.EqualTo(ValidFirstName));
            Assert.That(person.LastName, Is.EqualTo(ValidLastName));
            Assert.That(person.Email, Is.EqualTo(ValidEmail));
            Assert.That(person.IdentityNumber, Is.EqualTo(ValidIdentityNumber));
            Assert.That(person.BirthDate, Is.EqualTo(birthDate));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a Person with null email throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Null email throws ArgumentException")]
    public void Constructor_NullEmail_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        string? nullEmail = null;

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, nullEmail!, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("email"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a Person with null firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Null firstName throws ArgumentException")]
    public void Constructor_NullFirstName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        string? nullFirstName = null;

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, nullFirstName!, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("firstName"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Person with null lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Null lastName throws ArgumentException")]
    public void Constructor_NullLastName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        string? nullLastName = null;

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, nullLastName!, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("lastName"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Person with null identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Null identityNumber throws ArgumentException")]
    public void Constructor_NullIdentityNumber_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        string? nullIdentityNumber = null;

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, nullIdentityNumber!, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("identityNumber"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Person with future BirthDate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Future birthDate throws ArgumentException")]
    public void Constructor_FutureBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var futureBirthDate = DateTime.UtcNow.AddYears(1);

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, futureBirthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("birthDate"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Person with empty email throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Empty email throws ArgumentException")]
    public void Constructor_EmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var emptyEmail = "";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, emptyEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("email"));
    }

    /// <summary>
    /// Domain-008: Verify that creating a Person with empty firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Empty firstName throws ArgumentException")]
    public void Constructor_EmptyFirstName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var emptyFirstName = "";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, emptyFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("firstName"));
    }

    /// <summary>
    /// Domain-009: Verify that creating a Person with empty lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-009: Empty lastName throws ArgumentException")]
    public void Constructor_EmptyLastName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var emptyLastName = "";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, emptyLastName, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("lastName"));
    }

    /// <summary>
    /// Domain-010: Verify that creating a Person with empty identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-010: Empty identityNumber throws ArgumentException")]
    public void Constructor_EmptyIdentityNumber_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var emptyIdentityNumber = "";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, emptyIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("identityNumber"));
    }

    /// <summary>
    /// Domain-011: Verify that a Person can be created with optional Phone field provided.
    /// </summary>
    [Test]
    [Description("Domain-011: Person created with optional Phone field has correct Phone value")]
    public void Constructor_WithPhoneOptionally_PhonePropertySetCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);

        // Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate, ValidPhone);

        // Assert
        Assert.That(person.Phone, Is.EqualTo(ValidPhone));
    }

    /// <summary>
    /// Domain-012: Verify that a Person can be created with null Phone (optional field).
    /// </summary>
    [Test]
    [Description("Domain-012: Person created with null Phone has Phone property null")]
    public void Constructor_WithNullPhone_PhonePropertyIsNull()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        string? nullPhone = null;

        // Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate, nullPhone);

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
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var whitespaceEmail = "   ";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, whitespaceEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("email"));
    }

    /// <summary>
    /// Domain-014: Verify that creating a Person with whitespace-only firstName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-014: Whitespace-only firstName throws ArgumentException")]
    public void Constructor_WhitespaceFirstName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var whitespaceFirstName = "   ";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, whitespaceFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("firstName"));
    }

    /// <summary>
    /// Domain-015: Verify that creating a Person with whitespace-only lastName throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-015: Whitespace-only lastName throws ArgumentException")]
    public void Constructor_WhitespaceLastName_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var whitespaceLastName = "   ";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, whitespaceLastName, ValidEmail, ValidIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("lastName"));
    }

    /// <summary>
    /// Domain-016: Verify that creating a Person with whitespace-only identityNumber throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-016: Whitespace-only identityNumber throws ArgumentException")]
    public void Constructor_WhitespaceIdentityNumber_ThrowsArgumentException()
    {
        // Arrange
        var birthDate = new DateTime(1990, 1, 1);
        var whitespaceIdentityNumber = "   ";

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, whitespaceIdentityNumber, birthDate));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("identityNumber"));
    }

    /// <summary>
    /// Domain-017: Verify that creating a Person with today's date as BirthDate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-017: Today's date as birthDate throws ArgumentException")]
    public void Constructor_TodayAsBirthDate_ThrowsArgumentException()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;

        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, today));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("birthDate"));
    }
}
