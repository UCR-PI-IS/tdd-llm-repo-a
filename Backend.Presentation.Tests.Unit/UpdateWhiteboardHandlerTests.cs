using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005 for story CPD-LC-001-005.
/// </summary>
[TestFixture]
public class UpdateWhiteboardHandlerTests
{
    private Mock<IWhiteboardService> _mockService = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "LS-0103";

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

    private static UpdateWhiteboardRequest CreateUpdateRequest(
        string? componentId = null,
        float width = 3.0f,
        float height = 2.0f,
        float depth = 0.2f,
        float x = 2.0f,
        float y = 1.0f,
        float z = 3.0f,
        string orientation = "South",
        string markerColor = "Red")
    {
        return new UpdateWhiteboardRequest(
            componentId ?? ValidComponentId,
            width, height, depth, x, y, z,
            orientation, markerColor);
    }

    /// <summary>
    /// Presentation-001: Verify that the handler returns 200 OK with updated whiteboard data
    /// when the update is successful.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with updated whiteboard when update succeeds")]
    public async Task HandleAsync_SuccessfulUpdate_ReturnsOkWithUpdatedWhiteboard()
    {
        // Arrange
        var updatedWhiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Success(updatedWhiteboard));

        var request = CreateUpdateRequest();

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

        var request = CreateUpdateRequest(componentId: "WB-NONEXISTENT");

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
    /// Presentation-003: Verify that the handler returns 400 Bad Request
    /// when validation fails due to an invalid markerColor.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 Bad Request when markerColor is invalid")]
    public async Task HandleAsync_InvalidMarkerColor_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Invalid marker color"));

        var request = CreateUpdateRequest(markerColor: "");

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
    /// Presentation-004: Verify that the handler returns 400 Bad Request
    /// when the position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns 400 Bad Request when position exceeds boundaries")]
    public async Task HandleAsync_PositionExceedsBoundaries_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position exceeds learning space boundaries"));

        var request = CreateUpdateRequest(
            width: 2.0f, height: 1.5f, depth: 0.1f,
            x: 100.0f, y: 0.0f, z: 100.0f,
            orientation: "North", markerColor: "Blue");

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
    /// Presentation-005: Verify that the handler returns 400 Bad Request
    /// when the position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 400 Bad Request when position overlaps with existing component")]
    public async Task HandleAsync_PositionOverlaps_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardResult.Failure("Position overlaps with existing component"));

        var request = CreateUpdateRequest(
            width: 2.0f, height: 1.5f, depth: 0.1f,
            x: 5.0f, y: 0.0f, z: 5.0f,
            orientation: "North", markerColor: "Blue");

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
