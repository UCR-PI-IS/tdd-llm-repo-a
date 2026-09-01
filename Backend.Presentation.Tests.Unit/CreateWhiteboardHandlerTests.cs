using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005.
/// </summary>
[TestFixture]
public class CreateWhiteboardHandlerTests
{
    private Mock<IWhiteboardService> _mockService = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "North";
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

    /// <summary>
    /// Presentation-001: Verify handler returns 200 OK with created whiteboard when request is valid.
    /// </summary>
    [Test]
    [Description("Presentation-001: Return 200 OK with created whiteboard when request is valid")]
    public async Task HandleAsync_ValidInput_ReturnsOk()
    {
        // Arrange
        var createdWhiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, ValidMarkerColor))
            .ReturnsAsync(createdWhiteboard);

        var request = new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreateWhiteboardResponse>>());
            var okResult = (Ok<CreateWhiteboardResponse>)result.Result;
            Assert.That(okResult.Value.Whiteboard.MarkerColor, Is.EqualTo("Blue"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify handler returns 400 Bad Request when input validation fails
    /// (invalid orientation).
    /// </summary>
    [Test]
    [Description("Presentation-002: Return 400 Bad Request when input validation fails (invalid orientation)")]
    public async Task HandleAsync_InvalidOrientation_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                "InvalidOrientation", ValidMarkerColor))
            .ThrowsAsync(new ArgumentException("Invalid orientation"));

        var request = new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            "InvalidOrientation", ValidMarkerColor);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Invalid orientation"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify handler returns 404 Not Found when learning space doesn't exist.
    /// </summary>
    [Test]
    [Description("Presentation-003: Return 404 Not Found when learning space doesn't exist")]
    public async Task HandleAsync_LearningSpaceNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, ValidMarkerColor))
            .ThrowsAsync(new NotFoundException("Learning space not found"));

        var request = new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
            var notFoundResult = (NotFound<string>)result.Result;
            Assert.That(notFoundResult.Value, Does.Contain("Learning space not found"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify handler returns 400 Bad Request when whiteboard doesn't fit in learning space.
    /// </summary>
    [Test]
    [Description("Presentation-004: Return 400 Bad Request when whiteboard doesn't fit in learning space")]
    public async Task HandleAsync_DoesNotFit_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, ValidMarkerColor))
            .ThrowsAsync(new ValidationException("Whiteboard does not fit"));

        var request = new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("does not fit"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify handler returns 500 Internal Server Error when unexpected exception occurs.
    /// </summary>
    [Test]
    [Description("Presentation-005: Return 500 Internal Server Error when unexpected exception occurs")]
    public async Task HandleAsync_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, ValidMarkerColor))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        var request = new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>());
            var problemResult = (ProblemHttpResult)result.Result;
            Assert.That(problemResult.ProblemDetails.Status, Is.EqualTo(500));
        });
    }
}
