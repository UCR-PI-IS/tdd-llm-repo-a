using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonHandler.HandleAsync"/>.
/// Covers intents Presentation-008 through Presentation-010.
/// </summary>
[TestFixture]
public class CreatePersonHandlerTests
{
    private Mock<IPersonService> _mockService = null!;
    private Mock<CreatePersonDtoValidator> _mockValidator = null!;
    private CreatePersonHandler _sut = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "ID-001";
    private static readonly DateTime ValidBirthDate = new(2000, 1, 1);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonService>();
        _mockValidator = new Mock<CreatePersonDtoValidator>();
        _sut = new CreatePersonHandler(_mockService.Object, _mockValidator.Object);
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

    /// <summary>
    /// Presentation-008: Verify that the handler returns 200 OK with a success response
    /// when person creation succeeds (valid DTO and no duplicates).
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with success response when person creation succeeds")]
    public async Task HandleAsync_ValidDtoAndNoDuplicates_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var dto = CreateValidDto();
        var validResult = new DtoValidationResult(true, new List<ValidationFailure>());
        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(validResult);
        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Success());

        // Act
        var result = await _sut.HandleAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreatePersonResponse>>());
            if (result.Result is Ok<CreatePersonResponse> okResult)
            {
                Assert.That(okResult.Value!.Success, Is.True);
            }
        });
    }

    /// <summary>
    /// Presentation-009: Verify that the handler returns 400 Bad Request with a
    /// ValidationErrorResponse when DTO validation fails.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest with validation errors when DTO validation fails")]
    public async Task HandleAsync_InvalidDto_ReturnsBadRequestWithValidationErrors()
    {
        // Arrange
        var dto = CreateValidDto();
        var invalidResult = new DtoValidationResult(false, new List<ValidationFailure>
        {
            new("FirstName", "FirstName is required")
        });
        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(invalidResult);

        // Act
        var result = await _sut.HandleAsync(dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify that the handler returns 409 Conflict with an ErrorResponse
    /// when the person already exists (duplicate email or identity number).
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task HandleAsync_DuplicatePerson_ReturnsConflictWithErrorMessage()
    {
        // Arrange
        var dto = CreateValidDto();
        var validResult = new DtoValidationResult(true, new List<ValidationFailure>());
        _mockValidator
            .Setup(v => v.Validate(dto))
            .Returns(validResult);
        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Failure("A person with this email already exists."));

        // Act
        var result = await _sut.HandleAsync(dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            if (result.Result is Conflict<ErrorResponse> conflictResult)
            {
                Assert.That(conflictResult.Value!.Message, Does.Contain("already exists"));
            }
        });
    }
}
