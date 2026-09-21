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
    private Mock<IPersonService> _mockService = null!;

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
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns success response when creation succeeds")]
    public async Task HandleAsync_ValidInput_ReturnsSuccessResponse()
    {
        // Arrange
        var successResult = new OperationResult<Person> { IsSuccess = true, Value = new Person(1, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate) };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>())).ReturnsAsync(successResult);

        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Ok<CreatePersonResponse>>());
        var okResult = result.Result as Ok<CreatePersonResponse>;
        Assert.That(okResult!.Value!.Success, Is.True);
    }

    /// <summary>
    /// Presentation-009: Verify handler returns validation error response when DTO validation fails.
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns validation error when DTO validation fails")]
    public async Task HandleAsync_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePersonDto("", ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists (duplicate email).
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns conflict error when person already exists")]
    public async Task HandleAsync_DuplicatePerson_ReturnsConflict()
    {
        // Arrange
        var failureResult = new OperationResult<Person> { IsSuccess = false, ErrorMessage = "A person with this email already exists." };
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>())).ReturnsAsync(failureResult);

        var dto = new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            var conflictResult = result.Result as Conflict<ErrorResponse>;
            Assert.That(conflictResult!.Value!.Message, Does.Contain("already exists"));
        });
    }
}
