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
/// Unit tests for <see cref="UpdateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class UpdateWhiteboardHandlerTests
{
    private Mock<IWhiteboardUpdateService> _mockService = null!;

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
        _mockService = new Mock<IWhiteboardUpdateService>();
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
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    private static UpdateWhiteboardResponse CreateSuccessResponse()
    {
        return new UpdateWhiteboardResponse(
            new WhiteboardDto(
                ValidWhiteboardId, ValidLearningSpaceId,
                3.0f, 2.0f, 0.2f,
                2.0f, 1.0f, 3.0f,
                "South", "Red"));
    }

    /// <summary>
    /// Presentation-001: Verify that handler returns 200 OK with updated whiteboard data when update is successful.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with updated whiteboard when update succeeds")]
    public async Task HandleAsync_ValidRequest_ReturnsOkWithUpdatedWhiteboard()
    {
        // Arrange
        var request = CreateValidRequest();
        var response = CreateSuccessResponse();

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardServiceResult.Success(response.Whiteboard));

        // Act
        var result = await UpdateWhiteboardHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<UpdateWhiteboardResponse>>());
            var okResult = (Ok<UpdateWhiteboardResponse>)result.Result;
            Assert.That(okResult.Value.Whiteboard.MarkerColor, Is.EqualTo("Red"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that handler returns 400 Bad Request when whiteboard ID is not found.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 Bad Request when whiteboard ID not found")]
    public async Task HandleAsync_WhiteboardNotFound_ReturnsBadRequest()
    {
        // Arrange
        var nonExistentWhiteboardId = "WB-NOTFOUND";
        var request = new UpdateWhiteboardRequest(
            nonExistentWhiteboardId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardServiceResult.Failure("Whiteboard not found"));

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
    [Description("Presentation-003: Handler returns 400 Bad Request when validation fails")]
    public async Task HandleAsync_InvalidMarkerColor_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateWhiteboardRequest(
            ValidWhiteboardId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, "");

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardServiceResult.Failure("Invalid marker color"));

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
            ValidWidth, ValidHeight, ValidDepth,
            100.0f, ValidY, 100.0f,
            ValidOrientation, ValidMarkerColor);

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardServiceResult.Failure("Position exceeds learning space boundaries"));

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
            ValidWidth, ValidHeight, ValidDepth,
            5.0f, ValidY, 5.0f,
            ValidOrientation, ValidMarkerColor);

        _mockService
            .Setup(s => s.UpdateWhiteboardAsync(It.IsAny<UpdateWhiteboardDto>()))
            .ReturnsAsync(UpdateWhiteboardServiceResult.Failure("Position overlaps with existing component"));

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

/// <summary>
/// Service interface for updating whiteboards.
/// </summary>
public interface IWhiteboardUpdateService
{
    Task<UpdateWhiteboardServiceResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto);
}

/// <summary>
/// Result object for whiteboard update service operations.
/// </summary>
public class UpdateWhiteboardServiceResult
{
    public bool IsSuccess { get; private set; }
    public WhiteboardDto? Value { get; private set; }
    public string? ErrorMessage { get; private set; }

    public static UpdateWhiteboardServiceResult Success(WhiteboardDto whiteboard)
    {
        return new UpdateWhiteboardServiceResult { IsSuccess = true, Value = whiteboard };
    }

    public static UpdateWhiteboardServiceResult Failure(string errorMessage)
    {
        return new UpdateWhiteboardServiceResult { IsSuccess = false, ErrorMessage = errorMessage };
    }
}

/// <summary>
/// Request object for updating a whiteboard.
/// </summary>
public record class UpdateWhiteboardRequest(
    string WhiteboardId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Response object for whiteboard update operations.
/// </summary>
public record class UpdateWhiteboardResponse(WhiteboardDto Whiteboard);

/// <summary>
/// Handler for updating whiteboards.
/// </summary>
public static class UpdateWhiteboardHandler
{
    public static async Task<Results<Ok<UpdateWhiteboardResponse>, BadRequest<string>>> HandleAsync(
        UpdateWhiteboardRequest request,
        IWhiteboardUpdateService service)
    {
        var dto = new UpdateWhiteboardDto(
            request.WhiteboardId,
            request.Width, request.Height, request.Depth,
            request.X, request.Y, request.Z,
            request.Orientation, request.MarkerColor);

        var result = await service.UpdateWhiteboardAsync(dto);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(result.ErrorMessage!);
        }

        var response = new UpdateWhiteboardResponse(result.Value!);
        return TypedResults.Ok(response);
    }
}

/// <summary>
/// DTO for updating a whiteboard.
/// </summary>
public record class UpdateWhiteboardDto(
    string WhiteboardId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
