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
    private const string ValidPhone = "+506-8888-8888";

    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

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
    public void Constructor_WithNullPhone_PhonePropertyIsNull()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }

    /// <summary>
    /// Provides test cases for null required string field validation.
    /// Each test case passes null for one field while keeping all others valid.
    /// </summary>
    private static IEnumerable<TestCaseData> NullRequiredFieldCases()
    {
        yield return new TestCaseData(null, ValidLastName, ValidEmail, ValidIdentityNumber, "firstName")
            .SetArgDisplayNames("null firstName")
            .SetDescription("Domain-003: Null firstName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, null, ValidEmail, ValidIdentityNumber, "lastName")
            .SetArgDisplayNames("null lastName")
            .SetDescription("Domain-004: Null lastName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, null, ValidIdentityNumber, "email")
            .SetArgDisplayNames("null email")
            .SetDescription("Domain-002: Null email throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, ValidEmail, null, "identityNumber")
            .SetArgDisplayNames("null identityNumber")
            .SetDescription("Domain-005: Null identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-002 through Domain-005: Verify that creating a Person with a null required
    /// string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(NullRequiredFieldCases))]
    public void Constructor_NullRequiredStringField_ThrowsArgumentException(
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
    /// Provides test cases for empty required string field validation.
    /// Each test case passes an empty string for one field while keeping all others valid.
    /// </summary>
    private static IEnumerable<TestCaseData> EmptyRequiredFieldCases()
    {
        yield return new TestCaseData("", ValidLastName, ValidEmail, ValidIdentityNumber, "firstName")
            .SetArgDisplayNames("empty firstName")
            .SetDescription("Domain-008: Empty firstName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, "", ValidEmail, ValidIdentityNumber, "lastName")
            .SetArgDisplayNames("empty lastName")
            .SetDescription("Domain-009: Empty lastName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, "", ValidIdentityNumber, "email")
            .SetArgDisplayNames("empty email")
            .SetDescription("Domain-007: Empty email throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, ValidEmail, "", "identityNumber")
            .SetArgDisplayNames("empty identityNumber")
            .SetDescription("Domain-010: Empty identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-007 through Domain-010: Verify that creating a Person with an empty required
    /// string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(EmptyRequiredFieldCases))]
    public void Constructor_EmptyRequiredStringField_ThrowsArgumentException(
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
    /// Provides test cases for whitespace-only required string field validation.
    /// Each test case passes a whitespace-only string for one field while keeping all others valid.
    /// </summary>
    private static IEnumerable<TestCaseData> WhitespaceRequiredFieldCases()
    {
        yield return new TestCaseData("   ", ValidLastName, ValidEmail, ValidIdentityNumber, "firstName")
            .SetArgDisplayNames("whitespace firstName")
            .SetDescription("Domain-014: Whitespace-only firstName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, "   ", ValidEmail, ValidIdentityNumber, "lastName")
            .SetArgDisplayNames("whitespace lastName")
            .SetDescription("Domain-015: Whitespace-only lastName throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, "   ", ValidIdentityNumber, "email")
            .SetArgDisplayNames("whitespace email")
            .SetDescription("Domain-013: Whitespace-only email throws ArgumentException");
        yield return new TestCaseData(ValidFirstName, ValidLastName, ValidEmail, "   ", "identityNumber")
            .SetArgDisplayNames("whitespace identityNumber")
            .SetDescription("Domain-016: Whitespace-only identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-013 through Domain-016: Verify that creating a Person with a whitespace-only
    /// required string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(WhitespaceRequiredFieldCases))]
    public void Constructor_WhitespaceRequiredStringField_ThrowsArgumentException(
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
    /// throws ArgumentException with the correct parameter name.
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
    public void Constructor_TodayAsBirthDate_ThrowsArgumentException()
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
}
