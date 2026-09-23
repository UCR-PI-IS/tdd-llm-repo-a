using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Person"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-017 for SPT-UM-001-003.
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
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);
    private const string ValidPhone = "+506-8888-8888";

    /// <summary>
    /// Domain-001: Verify that a Person entity can be created with all valid required fields
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Valid Person creation with all required fields")]
    public void Constructor_ValidRequiredFields_AllPropertiesSetCorrectly()
    {
        // Arrange
        var id = ValidId;
        var firstName = ValidFirstName;
        var lastName = ValidLastName;
        var email = ValidEmail;
        var identityNumber = ValidIdentityNumber;
        var birthDate = ValidBirthDate;

        // Act
        var person = new Person(id, firstName, lastName, email, identityNumber, birthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(person.Id, Is.EqualTo(id));
            Assert.That(person.FirstName, Is.EqualTo(firstName));
            Assert.That(person.LastName, Is.EqualTo(lastName));
            Assert.That(person.Email, Is.EqualTo(email));
            Assert.That(person.IdentityNumber, Is.EqualTo(identityNumber));
            Assert.That(person.BirthDate, Is.EqualTo(birthDate));
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
        // Arrange
        var phone = ValidPhone;

        // Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, phone);

        // Assert
        Assert.That(person.Phone, Is.EqualTo(phone));
    }

    /// <summary>
    /// Domain-012: Verify that a Person can be created with null Phone (optional field)
    /// and the Phone property is null.
    /// </summary>
    [Test]
    [Description("Domain-012: Valid Person creation with null Phone (optional field)")]
    public void Constructor_WithNullPhone_PhonePropertyIsNull()
    {
        // Arrange
        string? phone = null;

        // Act
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate, phone);

        // Assert
        Assert.That(person.Phone, Is.Null);
    }

    /// <summary>
    /// Provides test cases for null required field validation.
    /// Each test case specifies which field is null and the expected parameter name in the exception.
    /// </summary>
    public static IEnumerable<TestCaseData> NullFieldTestCases()
    {
        yield return new TestCaseData("email")
            .SetDescription("Domain-002: Null email throws ArgumentException");
        yield return new TestCaseData("firstName")
            .SetDescription("Domain-003: Null firstName throws ArgumentException");
        yield return new TestCaseData("lastName")
            .SetDescription("Domain-004: Null lastName throws ArgumentException");
        yield return new TestCaseData("identityNumber")
            .SetDescription("Domain-005: Null identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-002 through Domain-005: Verify that creating a Person with a null required field
    /// throws ArgumentException with the appropriate parameter name.
    /// </summary>
    [TestCaseSource(nameof(NullFieldTestCases))]
    public void Constructor_NullRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange
        var id = ValidId;
        var firstName = ValidFirstName;
        var lastName = ValidLastName;
        var email = ValidEmail;
        var identityNumber = ValidIdentityNumber;
        var birthDate = ValidBirthDate;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _ = fieldName switch
            {
                "email" => new Person(id, firstName, lastName, null!, identityNumber, birthDate),
                "firstName" => new Person(id, null!, lastName, email, identityNumber, birthDate),
                "lastName" => new Person(id, firstName, null!, email, identityNumber, birthDate),
                "identityNumber" => new Person(id, firstName, lastName, email, null!, birthDate),
                _ => throw new InvalidOperationException($"Unknown field: {fieldName}")
            };
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
    /// throws ArgumentException with parameter name "birthDate".
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
    /// Provides test cases for empty required field validation.
    /// Each test case specifies which field is empty and the expected parameter name in the exception.
    /// </summary>
    public static IEnumerable<TestCaseData> EmptyFieldTestCases()
    {
        yield return new TestCaseData("email")
            .SetDescription("Domain-007: Empty email throws ArgumentException");
        yield return new TestCaseData("firstName")
            .SetDescription("Domain-008: Empty firstName throws ArgumentException");
        yield return new TestCaseData("lastName")
            .SetDescription("Domain-009: Empty lastName throws ArgumentException");
        yield return new TestCaseData("identityNumber")
            .SetDescription("Domain-010: Empty identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-007 through Domain-010: Verify that creating a Person with an empty required field
    /// throws ArgumentException with the appropriate parameter name.
    /// </summary>
    [TestCaseSource(nameof(EmptyFieldTestCases))]
    public void Constructor_EmptyRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange
        var id = ValidId;
        var firstName = ValidFirstName;
        var lastName = ValidLastName;
        var email = ValidEmail;
        var identityNumber = ValidIdentityNumber;
        var birthDate = ValidBirthDate;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _ = fieldName switch
            {
                "email" => new Person(id, firstName, lastName, string.Empty, identityNumber, birthDate),
                "firstName" => new Person(id, string.Empty, lastName, email, identityNumber, birthDate),
                "lastName" => new Person(id, firstName, string.Empty, email, identityNumber, birthDate),
                "identityNumber" => new Person(id, firstName, lastName, email, string.Empty, birthDate),
                _ => throw new InvalidOperationException($"Unknown field: {fieldName}")
            };
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
    /// Provides test cases for whitespace-only required field validation.
    /// Each test case specifies which field is whitespace-only and the expected parameter name in the exception.
    /// </summary>
    public static IEnumerable<TestCaseData> WhitespaceFieldTestCases()
    {
        yield return new TestCaseData("email")
            .SetDescription("Domain-013: Whitespace-only email throws ArgumentException");
        yield return new TestCaseData("firstName")
            .SetDescription("Domain-014: Whitespace-only firstName throws ArgumentException");
        yield return new TestCaseData("lastName")
            .SetDescription("Domain-015: Whitespace-only lastName throws ArgumentException");
        yield return new TestCaseData("identityNumber")
            .SetDescription("Domain-016: Whitespace-only identityNumber throws ArgumentException");
    }

    /// <summary>
    /// Domain-013 through Domain-016: Verify that creating a Person with a whitespace-only required field
    /// throws ArgumentException with the appropriate parameter name.
    /// </summary>
    [TestCaseSource(nameof(WhitespaceFieldTestCases))]
    public void Constructor_WhitespaceOnlyRequiredField_ThrowsArgumentException(string fieldName)
    {
        // Arrange
        var id = ValidId;
        var firstName = ValidFirstName;
        var lastName = ValidLastName;
        var email = ValidEmail;
        var identityNumber = ValidIdentityNumber;
        var birthDate = ValidBirthDate;
        var whitespaceValue = "   ";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            _ = fieldName switch
            {
                "email" => new Person(id, firstName, lastName, whitespaceValue, identityNumber, birthDate),
                "firstName" => new Person(id, whitespaceValue, lastName, email, identityNumber, birthDate),
                "lastName" => new Person(id, firstName, whitespaceValue, email, identityNumber, birthDate),
                "identityNumber" => new Person(id, firstName, lastName, email, whitespaceValue, birthDate),
                _ => throw new InvalidOperationException($"Unknown field: {fieldName}")
            };
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
    /// Domain-017: Verify that creating a Person with today's date as BirthDate
    /// throws ArgumentException with parameter name "birthDate" (must be a past date, not today).
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
}
