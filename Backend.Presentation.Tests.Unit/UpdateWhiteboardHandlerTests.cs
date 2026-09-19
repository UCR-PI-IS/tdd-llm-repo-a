using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class UpdateWhiteboardHandlerTests
{
    private Mock<IWhiteboardService> _mockService = null!;

    // Valid test data
    private const string ValidWhiteboardId = "WB-001";
    private const string ValidLearningSpaceId = "LS-001";
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
            ValidWhiteboardId, 3.0f, 2.0f, 0.2f, 2.0f, 1.0f, 3.0f, "East", "Red");
    }

    /// <summary>
    /// Presentation-001: Verify that the handler returns 200 OK with updated whiteboard data
    /// when the update is successful.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with updated whiteboard when successful")]
    public async Task HandleAsync_SuccessfulUpdate_ReturnsOkWithUpdatedData()
    {
        // Arrange
        var updatedWhiteboard = new Whiteboard(
            ValidWhiteboardId, ValidLearningSpaceId,
            3.0f, 2.0f, 0.2f, 2.0f, 1.0f, 3.0f,
            "East", "Red");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Success(updatedWhiteboard));

        var request = CreateValidRequest();

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<UpdateWhiteboardResponse>>());
            var okResult = result.Result as Ok<UpdateWhiteboardResponse>;
            Assert.That(okResult!.Value.MarkerColor, Is.EqualTo("Red"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that the handler returns 400 Bad Request
    /// when the whiteboard ID is not found.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 Bad Request when whiteboard not found")]
    public async Task HandleAsync_WhiteboardNotFound_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Whiteboard not found"));

        var request = CreateValidRequest();

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("Whiteboard not found"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that the handler returns 400 Bad Request
    /// when validation fails due to an invalid marker color.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 Bad Request when marker color is invalid")]
    public async Task HandleAsync_InvalidMarkerColor_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Invalid marker color"));

        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId, 3.0f, 2.0f, 0.2f, 2.0f, 1.0f, 3.0f, "East", "");

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("Invalid marker color"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that the handler returns 400 Bad Request
    /// when the new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns 400 Bad Request when position exceeds boundaries")]
    public async Task HandleAsync_PositionExceedsBoundaries_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position exceeds learning space boundaries"));

        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId, ValidWidth, ValidHeight, ValidDepth, 100.0f, ValidY, 100.0f, ValidOrientation, ValidMarkerColor);

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("Position exceeds learning space boundaries"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that the handler returns 400 Bad Request
    /// when the new position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 400 Bad Request when position overlaps with component")]
    public async Task HandleAsync_PositionOverlapsComponent_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position overlaps with existing component"));

        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId, ValidWidth, ValidHeight, ValidDepth, 5.0f, ValidY, 5.0f, ValidOrientation, ValidMarkerColor);

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("Position overlaps with existing component"));
        });
    }
}
