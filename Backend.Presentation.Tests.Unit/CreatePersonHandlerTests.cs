using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonHandler.HandleAsync"/>.
/// Covers intents Presentation-008 through Presentation-010 for SPT-UM-001-003.
/// </summary>
[TestFixture]
public class CreatePersonHandlerTests
{
    private Mock<IPersonCreateService> _mockService = null!;
    private Mock<IValidator<CreatePersonDto>> _mockValidator = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonCreateService>();
        _mockValidator = new Mock<IValidator<CreatePersonDto>>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
        _mockValidator.VerifyAll();
    }

    private static CreatePersonDto CreateValidDto()
    {
        return new CreatePersonDto(
            ValidFirstName,
            ValidLastName,
            ValidEmail,
            ValidIdentityNumber,
            ValidBirthDate);
    }

    private void SetupValidValidation()
    {
        _mockValidator
            .Setup(v => v.Validate(It.IsAny<CreatePersonDto>()))
            .Returns(new ValidationResult());
    }

    private void SetupInvalidValidation(string propertyName, string errorMessage)
    {
        _mockValidator
            .Setup(v => v.Validate(It.IsAny<CreatePersonDto>()))
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure(propertyName, errorMessage)
            }));
    }

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// The validator passes, the service returns success, and the handler returns Ok with CreatePersonResponse.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with success response when person creation succeeds")]
    public async Task HandleAsync_ValidDto_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var dto = CreateValidDto();
        SetupValidValidation();

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Success());

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, _mockValidator.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreatePersonResponse>>());
            var okResult = result.Result as Ok<CreatePersonResponse>;
            Assert.That(okResult!.Value!.Success, Is.True);
        });
    }

    /// <summary>
    /// Presentation-009: Verify handler returns validation error response when DTO validation fails.
    /// The validator returns errors, and the handler returns BadRequest with ValidationErrorResponse.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest when DTO validation fails")]
    public async Task HandleAsync_ValidationFails_ReturnsBadRequest()
    {
        // Arrange
        var dto = CreateValidDto();
        SetupInvalidValidation("FirstName", "First name is required");

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, _mockValidator.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
        _mockService.Verify(s => s.CreatePersonAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists (duplicate email).
    /// The validator passes, the service returns a failure with "already exists" message,
    /// and the handler returns Conflict with ErrorResponse.
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task HandleAsync_DuplicatePerson_ReturnsConflict()
    {
        // Arrange
        var dto = CreateValidDto();
        SetupValidValidation();

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Failure("A person with this email already exists."));

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, _mockValidator.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            var conflictResult = result.Result as Conflict<ErrorResponse>;
            Assert.That(conflictResult!.Value!.Message, Does.Contain("already exists"));
        });
    }
}
