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

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IUniversityService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify that successful university creation returns 200 OK with success message.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns Ok with success message when creation succeeds")]
    public async Task HandleAsync_SuccessfulCreation_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var name = "UCR";
        var country = "Costa Rica";

        _mockService
            .Setup(s => s.AddUniversityAsync(name, country))
            .ReturnsAsync(Result.Success());

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<AddUniversityResponse>>());
            var okResult = result.Result as Ok<AddUniversityResponse>;
            Assert.That(okResult!.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns BadRequest when university already exists")]
    public async Task HandleAsync_DuplicateName_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        var name = "UCR";
        var country = "Costa Rica";

        _mockService
            .Setup(s => s.AddUniversityAsync(name, country))
            .ReturnsAsync(Result.Failure("University with name 'UCR' already exists"));

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format returns 400 BadRequest with validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns BadRequest when property validation fails")]
    public async Task HandleAsync_InvalidName_ReturnsBadRequestWithValidationError()
    {
        // Arrange
        var name = "";
        var country = "Costa Rica";

        _mockService
            .Setup(s => s.AddUniversityAsync(name, country))
            .ReturnsAsync(Result.Failure("Name is required"));

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("required"));
        });
    }
}
