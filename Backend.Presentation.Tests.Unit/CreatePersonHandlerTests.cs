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
/// Covers intents Presentation-008 through Presentation-010.
/// </summary>
[TestFixture]
public class CreatePersonHandlerTests
{
    private Mock<IPersonCreateService> _mockService = null!;

    // Valid test data
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonCreateService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
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

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// The handler should return Ok with a CreatePersonResponse where Success is true.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with success response when person creation succeeds")]
    public async Task HandleAsync_ServiceReturnsSuccess_ReturnsOkWithCreatePersonResponse()
    {
        // Arrange
        var dto = CreateValidDto();
        var successResult = CreatePersonResult.Success(new Person(1, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate));

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(successResult);

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
    /// The handler should return BadRequest with a ValidationErrorResponse.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest when DTO validation fails")]
    public async Task HandleAsync_ValidationFails_ReturnsBadRequestWithValidationErrorResponse()
    {
        // Arrange
        var dto = new CreatePersonDto(
            string.Empty,
            ValidLastName,
            ValidEmail,
            ValidIdentityNumber,
            ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists (duplicate email).
    /// The handler should return Conflict with an ErrorResponse containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task HandleAsync_PersonAlreadyExists_ReturnsConflictWithErrorResponse()
    {
        // Arrange
        var dto = CreateValidDto();
        var failureResult = CreatePersonResult.Failure("A person with this email already exists.");

        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(failureResult);

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
}
