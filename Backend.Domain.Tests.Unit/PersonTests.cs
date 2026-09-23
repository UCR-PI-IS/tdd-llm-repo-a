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
    private const string ValidIdentityNumber = "ID-001";
    private static readonly DateTime ValidBirthDate = new(2000, 1, 1);
    private const string ValidPhone = "+1234567890";

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Person created with all valid required fields has correct property values")]
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
    /// Domain-011: Verify that a Person can be created with the optional Phone field provided.
    /// </summary>
    [Test]
    [Description("Domain-011: Person created with optional Phone field has Phone property set")]
    public void Constructor_WithOptionalPhone_PhonePropertySet()
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
    [Description("Domain-012: Person created with null Phone has Phone property as null")]
    public void Constructor_WithNullPhone_PhonePropertyIsNull()
    {
        // Arrange & Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, null);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }

    /// <summary>
    /// Domain-002, Domain-003, Domain-004, Domain-005: Verify that creating a Person with a null
    /// required string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(NullRequiredFieldCases))]
    [Description("Domain-002 to Domain-005: Null required field throws ArgumentException")]
    public void Constructor_NullRequiredField_ThrowsArgumentException(
        int id, string? firstName, string? lastName, string? email,
        string? identityNumber, DateTime birthDate, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(id, firstName!, lastName!, email!, identityNumber!, birthDate);
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
    /// Domain-007, Domain-008, Domain-009, Domain-010: Verify that creating a Person with an empty
    /// required string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(EmptyRequiredFieldCases))]
    [Description("Domain-007 to Domain-010: Empty required field throws ArgumentException")]
    public void Constructor_EmptyRequiredField_ThrowsArgumentException(
        int id, string firstName, string lastName, string email,
        string identityNumber, DateTime birthDate, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(id, firstName, lastName, email, identityNumber, birthDate);
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
    /// Domain-013, Domain-014, Domain-015, Domain-016: Verify that creating a Person with a
    /// whitespace-only required string field throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(WhitespaceRequiredFieldCases))]
    [Description("Domain-013 to Domain-016: Whitespace-only required field throws ArgumentException")]
    public void Constructor_WhitespaceRequiredField_ThrowsArgumentException(
        int id, string firstName, string lastName, string email,
        string identityNumber, DateTime birthDate, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(id, firstName, lastName, email, identityNumber, birthDate);
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
    /// Domain-006, Domain-017: Verify that creating a Person with a future or today's date
    /// as BirthDate throws ArgumentException with paramName "birthDate".
    /// </summary>
    [TestCaseSource(nameof(InvalidBirthDateCases))]
    [Description("Domain-006 and Domain-017: Future or today BirthDate throws ArgumentException")]
    public void Constructor_InvalidBirthDate_ThrowsArgumentException(
        int id, string firstName, string lastName, string email,
        string identityNumber, DateTime birthDate, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Person(id, firstName, lastName, email, identityNumber, birthDate);
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

    #region TestCaseSources

    /// <summary>
    /// Test cases for null required fields (Domain-002, Domain-003, Domain-004, Domain-005).
    /// </summary>
    public static IEnumerable<TestCaseData> NullRequiredFieldCases
    {
        get
        {
            var validDate = new DateTime(2000, 1, 1);
            yield return new TestCaseData(1, null, "Doe", "john@example.com", "ID-001", validDate, "firstName")
                .SetDescription("Domain-003: null firstName throws ArgumentException");
            yield return new TestCaseData(1, "John", null, "john@example.com", "ID-001", validDate, "lastName")
                .SetDescription("Domain-004: null lastName throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", null, "ID-001", validDate, "email")
                .SetDescription("Domain-002: null email throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", "john@example.com", null, validDate, "identityNumber")
                .SetDescription("Domain-005: null identityNumber throws ArgumentException");
        }
    }

    /// <summary>
    /// Test cases for empty required fields (Domain-007, Domain-008, Domain-009, Domain-010).
    /// </summary>
    public static IEnumerable<TestCaseData> EmptyRequiredFieldCases
    {
        get
        {
            var validDate = new DateTime(2000, 1, 1);
            yield return new TestCaseData(1, "", "Doe", "john@example.com", "ID-001", validDate, "firstName")
                .SetDescription("Domain-008: empty firstName throws ArgumentException");
            yield return new TestCaseData(1, "John", "", "john@example.com", "ID-001", validDate, "lastName")
                .SetDescription("Domain-009: empty lastName throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", "", "ID-001", validDate, "email")
                .SetDescription("Domain-007: empty email throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", "john@example.com", "", validDate, "identityNumber")
                .SetDescription("Domain-010: empty identityNumber throws ArgumentException");
        }
    }

    /// <summary>
    /// Test cases for whitespace-only required fields (Domain-013, Domain-014, Domain-015, Domain-016).
    /// </summary>
    public static IEnumerable<TestCaseData> WhitespaceRequiredFieldCases
    {
        get
        {
            var validDate = new DateTime(2000, 1, 1);
            yield return new TestCaseData(1, "   ", "Doe", "john@example.com", "ID-001", validDate, "firstName")
                .SetDescription("Domain-014: whitespace firstName throws ArgumentException");
            yield return new TestCaseData(1, "John", "   ", "john@example.com", "ID-001", validDate, "lastName")
                .SetDescription("Domain-015: whitespace lastName throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", "   ", "ID-001", validDate, "email")
                .SetDescription("Domain-013: whitespace email throws ArgumentException");
            yield return new TestCaseData(1, "John", "Doe", "john@example.com", "   ", validDate, "identityNumber")
                .SetDescription("Domain-016: whitespace identityNumber throws ArgumentException");
        }
    }

    /// <summary>
    /// Test cases for invalid BirthDate values (Domain-006, Domain-017).
    /// </summary>
    public static IEnumerable<TestCaseData> InvalidBirthDateCases
    {
        get
        {
            yield return new TestCaseData(
                    1, "John", "Doe", "john@example.com", "ID-001",
                    DateTime.Today.AddDays(1), "birthDate")
                .SetDescription("Domain-006: future BirthDate throws ArgumentException");
            yield return new TestCaseData(
                    1, "John", "Doe", "john@example.com", "ID-001",
                    DateTime.Today, "birthDate")
                .SetDescription("Domain-017: today as BirthDate throws ArgumentException");
        }
    }

    #endregion
}
