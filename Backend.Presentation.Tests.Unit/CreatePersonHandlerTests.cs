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
    private const int ValidId = 1;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@email.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

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
    /// Presentation-008: Verify that the handler returns 200 OK with a CreatePersonResponse
    /// when person creation succeeds.
    /// </summary>
    [Test]
    [Description("Presentation-008: Handler returns Ok with CreatePersonResponse on success")]
    public async Task HandleAsync_Success_ReturnsOkWithResponse()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(PersonCreationResult.Success());

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
    /// Presentation-009: Verify that the handler returns 400 Bad Request with a ValidationErrorResponse
    /// when DTO validation fails (e.g., missing required fields).
    /// </summary>
    [Test]
    [Description("Presentation-009: Handler returns BadRequest when DTO validation fails")]
    public async Task HandleAsync_ValidationFails_ReturnsBadRequest()
    {
        // Arrange
        var invalidDto = new CreatePersonDto(
            "",
            ValidLastName,
            ValidEmail,
            ValidIdentityNumber,
            ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, invalidDto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify that the handler returns 409 Conflict with an ErrorResponse
    /// when the person already exists (duplicate email or identity number).
    /// </summary>
    [Test]
    [Description("Presentation-010: Handler returns Conflict when person already exists")]
    public async Task HandleAsync_PersonAlreadyExists_ReturnsConflict()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(PersonCreationResult.Failure("A person with this email already exists."));

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
}
