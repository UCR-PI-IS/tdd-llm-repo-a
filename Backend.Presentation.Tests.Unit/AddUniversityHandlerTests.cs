using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AddUniversityHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003.
/// </summary>
[TestFixture]
public class AddUniversityHandlerTests
{
    private Mock<IUniversityService> _mockService = null!;
    private AddUniversityHandler _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IUniversityService>();
        _sut = new AddUniversityHandler(_mockService.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify that successful university creation returns 200 OK
    /// with an AddUniversityResponse containing the success message.
    /// </summary>
    [Test]
    [Description("Presentation-001: Successful creation returns Ok with success message")]
    public async Task HandleAsync_SuccessfulCreation_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("UCR", "Costa Rica"))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync("UCR", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<Ok<AddUniversityResponse>>());
            var okResult = result.Result as Ok<AddUniversityResponse>;
            Assert.That(okResult!.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest
    /// with an error message containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate university returns BadRequest with 'already exists' message")]
    public async Task HandleAsync_DuplicateUniversity_ReturnsBadRequestWithAlreadyExistsMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("UCR", "Costa Rica"))
            .ReturnsAsync(Result.Failure("University with name 'UCR' already exists"));

        // Act
        var result = await _sut.HandleAsync("UCR", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format returns 400 BadRequest
    /// with a validation error message containing "required".
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid property format returns BadRequest with validation error")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequestWithValidationError()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("", "Costa Rica"))
            .ReturnsAsync(Result.Failure("Name is required"));

        // Act
        var result = await _sut.HandleAsync("", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("required"));
        });
    }
}
