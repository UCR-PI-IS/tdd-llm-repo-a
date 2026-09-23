using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDtoValidator.Validate"/>.
/// Covers intents Presentation-002 through Presentation-007.
/// </summary>
[TestFixture]
public class CreatePersonDtoValidatorTests
{
    private CreatePersonDtoValidator _sut = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "ID-001";
    private static readonly DateTime ValidBirthDate = new(2000, 1, 1);

    [SetUp]
    public void SetUp()
    {
        _sut = new CreatePersonDtoValidator();
    }

    private static CreatePersonDto CreateValidDto(
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? identityNumber = null,
        DateTime? birthDate = null)
    {
        return new CreatePersonDto(
            firstName ?? ValidFirstName,
            lastName ?? ValidLastName,
            email ?? ValidEmail,
            identityNumber ?? ValidIdentityNumber,
            birthDate ?? ValidBirthDate);
    }

    /// <summary>
    /// Presentation-002, Presentation-003, Presentation-004, Presentation-006:
    /// Verify that validation fails when a required field is null or empty.
    /// The result should contain a validation error for the specific property.
    /// </summary>
    [TestCaseSource(nameof(NullOrEmptyRequiredFieldCases))]
    [Description("Presentation-002, 003, 004, 006: Null or empty required field returns invalid validation result")]
    public void Validate_NullOrEmptyRequiredField_ReturnsInvalid(CreatePersonDto dto, string expectedProperty)
    {
        // Arrange & Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == expectedProperty));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that validation fails when the Email format is invalid.
    /// The result should contain a validation error for the Email property.
    /// </summary>
    [TestCase("not-an-email", Description = "Presentation-005: Plain string is not a valid email")]
    [TestCase("missing@domain", Description = "Presentation-005: Missing TLD is not a valid email")]
    [TestCase("@nodomain.com", Description = "Presentation-005: Missing local part is not a valid email")]
    public void Validate_InvalidEmailFormat_ReturnsInvalid(string invalidEmail)
    {
        // Arrange
        var dto = CreateValidDto(email: invalidEmail);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-007: Verify that validation fails when BirthDate is in the future.
    /// The result should contain a validation error for the BirthDate property.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate returns invalid validation result")]
    public void Validate_FutureBirthDate_ReturnsInvalid()
    {
        // Arrange
        var dto = CreateValidDto(birthDate: DateTime.Today.AddDays(1));

        // Act
        var result = _sut.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "BirthDate"));
        });
    }

    #region TestCaseSources

    /// <summary>
    /// Test cases for null or empty required fields (Presentation-002, 003, 004, 006).
    /// </summary>
    public static IEnumerable<TestCaseData> NullOrEmptyRequiredFieldCases
    {
        get
        {
            var validDate = new DateTime(2000, 1, 1);

            // Null cases
            yield return new TestCaseData(
                    new CreatePersonDto(null!, ValidLastName, ValidEmail, ValidIdentityNumber, validDate),
                    "FirstName")
                .SetDescription("Presentation-002: null FirstName returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, null!, ValidEmail, ValidIdentityNumber, validDate),
                    "LastName")
                .SetDescription("Presentation-003: null LastName returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, ValidLastName, null!, ValidIdentityNumber, validDate),
                    "Email")
                .SetDescription("Presentation-004: null Email returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, null!, validDate),
                    "IdentityNumber")
                .SetDescription("Presentation-006: null IdentityNumber returns invalid");

            // Empty cases
            yield return new TestCaseData(
                    new CreatePersonDto("", ValidLastName, ValidEmail, ValidIdentityNumber, validDate),
                    "FirstName")
                .SetDescription("Presentation-002: empty FirstName returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, "", ValidEmail, ValidIdentityNumber, validDate),
                    "LastName")
                .SetDescription("Presentation-003: empty LastName returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, ValidLastName, "", ValidIdentityNumber, validDate),
                    "Email")
                .SetDescription("Presentation-004: empty Email returns invalid");
            yield return new TestCaseData(
                    new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, "", validDate),
                    "IdentityNumber")
                .SetDescription("Presentation-006: empty IdentityNumber returns invalid");
        }
    }

    #endregion
}
