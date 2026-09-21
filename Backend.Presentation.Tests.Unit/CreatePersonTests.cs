using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;
using FluentValidation.Results;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonDto"/>, <see cref="CreatePersonDtoValidator"/>,
/// and <see cref="CreatePersonHandler"/>.
/// Covers intents Presentation-001 through Presentation-011.
/// </summary>
[TestFixture]
public class CreatePersonTests
{
    private Mock<IPersonService> _mockService = null!;
    private CreatePersonDtoValidator _validator = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonService>();
        _validator = new CreatePersonDtoValidator();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static CreatePersonDto CreateValidDto()
    {
        return new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
    }

    // ===== CreatePersonDto tests =====

    /// <summary>
    /// Presentation-001: Verify DTO can be created with all valid required fields.
    /// </summary>
    [Test]
    [Description("Presentation-001: DTO created with all valid required fields has correct property values")]
    public void CreatePersonDto_ValidFields_AllPropertiesSetCorrectly()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var identityNumber = "123456789";
        var birthDate = new DateTime(1990, 1, 1);

        // Act
        var dto = new CreatePersonDto(firstName, lastName, email, identityNumber, birthDate);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.FirstName, Is.EqualTo(firstName));
            Assert.That(dto.LastName, Is.EqualTo(lastName));
            Assert.That(dto.Email, Is.EqualTo(email));
            Assert.That(dto.IdentityNumber, Is.EqualTo(identityNumber));
            Assert.That(dto.BirthDate, Is.EqualTo(birthDate));
        });
    }

    // ===== CreatePersonDtoValidator tests =====

    /// <summary>
    /// Presentation-002: Verify validation fails when FirstName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-002: Null FirstName fails validation")]
    [TestCase("", Description = "Presentation-002: Empty FirstName fails validation")]
    public void CreatePersonDtoValidator_InvalidFirstName_ReturnsValidationError(string? invalidFirstName)
    {
        // Arrange
        var dto = new CreatePersonDto(invalidFirstName!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "FirstName"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify validation fails when LastName is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-003: Null LastName fails validation")]
    [TestCase("", Description = "Presentation-003: Empty LastName fails validation")]
    public void CreatePersonDtoValidator_InvalidLastName_ReturnsValidationError(string? invalidLastName)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, invalidLastName!, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "LastName"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify validation fails when Email is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-004: Null Email fails validation")]
    [TestCase("", Description = "Presentation-004: Empty Email fails validation")]
    public void CreatePersonDtoValidator_InvalidEmail_ReturnsValidationError(string? invalidEmail)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, invalidEmail!, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify validation fails when Email format is invalid.
    /// </summary>
    [Test]
    [Description("Presentation-005: Invalid email format fails validation")]
    public void CreatePersonDtoValidator_InvalidEmailFormat_ReturnsValidationError()
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, "invalid-email", ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "Email"));
        });
    }

    /// <summary>
    /// Presentation-006: Verify validation fails when IdentityNumber is null or empty.
    /// </summary>
    [TestCase(null, Description = "Presentation-006: Null IdentityNumber fails validation")]
    [TestCase("", Description = "Presentation-006: Empty IdentityNumber fails validation")]
    public void CreatePersonDtoValidator_InvalidIdentityNumber_ReturnsValidationError(string? invalidIdentityNumber)
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, invalidIdentityNumber!, ValidBirthDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "IdentityNumber"));
        });
    }

    /// <summary>
    /// Presentation-007: Verify validation fails when BirthDate is in the future.
    /// </summary>
    [Test]
    [Description("Presentation-007: Future BirthDate fails validation")]
    public void CreatePersonDtoValidator_FutureBirthDate_ReturnsValidationError()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddYears(1);
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, futureDate);

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "BirthDate"));
        });
    }

    // ===== CreatePersonHandler tests =====

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with success response when creation succeeds")]
    public async Task CreatePersonHandler_ValidRequest_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var person = new Person("PER-001", ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        var serviceResult = new OperationResult<Person> { IsSuccess = true, Value = person };

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(serviceResult);

        var dto = CreateValidDto();

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreatePersonResponse>>());
            var okResult = (Ok<CreatePersonResponse>)result.Result;
            Assert.That(okResult.Value!.Success, Is.True);
        });
    }

    /// <summary>
    /// Presentation-009: Verify handler returns validation error response when DTO validation fails.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest when DTO validation fails")]
    public async Task CreatePersonHandler_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePersonDto("", ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists.
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task CreatePersonHandler_DuplicatePerson_ReturnsConflict()
    {
        // Arrange
        var serviceResult = new OperationResult<Person>
        {
            IsSuccess = false,
            ErrorMessage = "A person with this email already exists."
        };

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(serviceResult);

        var dto = CreateValidDto();

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            var conflictResult = (Conflict<ErrorResponse>)result.Result;
            Assert.That(conflictResult.Value!.Message, Does.Contain("already exists"));
        });
    }

    // ===== CreatePersonEndpoint tests =====

    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is mapped to POST /api/persons")]
    public void CreatePersonEndpoint_MapEndpoint_RegistersPostApiPersons()
    {
        // Arrange
        var endpoints = new List<EndpointDataSource>();
        var mockBuilder = new Mock<IEndpointRouteBuilder>();
        mockBuilder.Setup(b => b.DataSources).Returns(endpoints);

        // Act
        CreatePersonEndpoint.MapEndpoint(mockBuilder.Object);

        // Assert
        var personEndpoint = endpoints
            .SelectMany(ds => ds.Endpoints)
            .FirstOrDefault(e => e.DisplayName?.Contains("CreatePerson") == true ||
                (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");
        Assert.That(personEndpoint, Is.Not.Null);
    }
}
