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
    private Mock<IPersonService> _mockService = null!;
    private Mock<IValidator<CreatePersonDto>> _mockValidator = null!;
    private CreatePersonHandler _sut = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new(1990, 1, 15);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonService>();
        _mockValidator = new Mock<IValidator<CreatePersonDto>>();
        _sut = new CreatePersonHandler(_mockService.Object, _mockValidator.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
        _mockValidator.VerifyAll();
    }

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// The handler should validate the DTO, call the service, and return Ok with a success response.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with success response when person creation succeeds")]
    public async Task HandleAsync_ValidDto_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(new CreatePersonResult { IsSuccess = true });

        // Act
        var result = await _sut.HandleAsync(dto);

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
    /// The handler should return BadRequest without calling the service.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest when DTO validation fails")]
    public async Task HandleAsync_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePersonDto("", ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult(new[]
            {
                new ValidationFailure("FirstName", "FirstName is required")
            }));

        // Act
        var result = await _sut.HandleAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
            _mockService.Verify(s => s.CreatePersonAsync(It.IsAny<Person>()), Times.Never);
        });
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists (duplicate email).
    /// The handler should return Conflict with an error message indicating duplication.
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task HandleAsync_DuplicatePerson_ReturnsConflict()
    {
        // Arrange
        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(new ValidationResult());

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(new CreatePersonResult
            {
                IsSuccess = false,
                ErrorMessage = "A person with this email already exists."
            });

        // Act
        var result = await _sut.HandleAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            var conflictResult = result.Result as Conflict<ErrorResponse>;
            Assert.That(conflictResult!.Value!.Message, Does.Contain("already exists"));
        });
    }
}
