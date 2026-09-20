using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Requests;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005.
/// </summary>
[TestFixture]
public class UpdateWhiteboardHandlerTests
{
    private Mock<IWhiteboardService> _mockService = null!;

    // Valid test data
    private const string ValidWhiteboardId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IWhiteboardService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static UpdateWhiteboardRequest CreateValidRequest()
    {
        return new UpdateWhiteboardRequest(
            ValidWhiteboardId,
            3.0f,
            2.0f,
            0.2f,
            2.0f,
            1.0f,
            3.0f,
            "East",
            "Red");
    }

    /// <summary>
    /// Presentation-001: Verify that handler returns 200 OK with updated whiteboard data when update is successful.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with updated whiteboard when successful")]
    public async Task HandleAsync_SuccessfulUpdate_ReturnsOkWithWhiteboard()
    {
        // Arrange
        var request = CreateValidRequest();
        var updatedWhiteboard = new Whiteboard(
            ValidWhiteboardId,
            ValidLearningSpaceId,
            3.0f,
            2.0f,
            0.2f,
            2.0f,
            1.0f,
            3.0f,
            "East",
            "Red");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Success(updatedWhiteboard));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<UpdateWhiteboardResponse>>());
            var okResult = (Ok<UpdateWhiteboardResponse>)result.Result;
            Assert.That(okResult.Value.MarkerColor, Is.EqualTo("Red"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that handler returns 400 Bad Request when whiteboard ID is not found.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 Bad Request when whiteboard not found")]
    public async Task HandleAsync_WhiteboardNotFound_ReturnsBadRequest()
    {
        // Arrange
        var nonExistentWhiteboardId = "WB-NOTFOUND";
        var request = new UpdateWhiteboardRequest(
            nonExistentWhiteboardId,
            3.0f,
            2.0f,
            0.2f,
            2.0f,
            1.0f,
            3.0f,
            "East",
            "Red");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Whiteboard not found"));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Whiteboard not found"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that handler returns 400 Bad Request when validation fails (invalid markerColor).
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 Bad Request when markerColor validation fails")]
    public async Task HandleAsync_InvalidMarkerColor_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId,
            3.0f,
            2.0f,
            0.2f,
            2.0f,
            1.0f,
            3.0f,
            "East",
            "");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Invalid marker color"));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Invalid marker color"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that handler returns 400 Bad Request when position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns 400 Bad Request when position exceeds boundaries")]
    public async Task HandleAsync_PositionExceedsBoundaries_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId,
            2.0f,
            1.5f,
            0.1f,
            100.0f,
            0.0f,
            100.0f,
            "North",
            "Blue");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position exceeds learning space boundaries"));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Position exceeds learning space boundaries"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that handler returns 400 Bad Request when position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 400 Bad Request when position overlaps with component")]
    public async Task HandleAsync_PositionOverlaps_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId,
            2.0f,
            1.5f,
            0.1f,
            5.0f,
            0.0f,
            5.0f,
            "North",
            "Blue");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position overlaps with existing component"));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Position overlaps with existing component"));
        });
    }
}
