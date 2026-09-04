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
/// Unit tests for <see cref="CreateWhiteboardHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-005.
/// </summary>
[TestFixture]
public class CreateWhiteboardHandlerTests
{
    private Mock<IWhiteboardService> _mockService = null!;

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
    public async Task HandleAsync_ValidRequest_ReturnsOkWithWhiteboard()
    {
        // Arrange
        var createdWhiteboard = new Whiteboard(
            "WB-001", "IF-0103", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        var request = new CreateWhiteboardDto(
            "IF-0103", "WB-001", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ReturnsAsync(createdWhiteboard);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Ok<CreateWhiteboardResponse>>())
;        var okResult = result.Result as Ok<CreateWhiteboardResponse>;
        Assert.That(okResult!.Value.Whiteboard.MarkerColor, Is.EqualTo("Blue"));
    }

    /// <summary>
    /// Presentation-002: Verify handler returns 400 Bad Request when input validation fails (invalid orientation).
    /// </summary>
    [Test]
    [Description("Presentation-002: Return 400 Bad Request when input validation fails")]
    public async Task HandleAsync_InvalidOrientation_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateWhiteboardDto(
            "IF-0103", "WB-001", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "InvalidOrientation", "Blue");

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ArgumentException("Invalid orientation"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>())
;        var badRequestResult = result.Result as BadRequest<string>;
        Assert.That(badRequestResult!.Value, Does.Contain("Invalid orientation"));
    }

    /// <summary>
    /// Presentation-003: Verify handler returns 404 Not Found when learning space doesn't exist.
    /// </summary>
    [Test]
    [Description("Presentation-003: Return 404 Not Found when learning space doesn't exist")]
    public async Task HandleAsync_LearningSpaceNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateWhiteboardDto(
            "NON-EXISTENT", "WB-001", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new NotFoundException("Learning space not found"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFound<string>>())
;        var notFoundResult = result.Result as NotFound<string>;
        Assert.That(notFoundResult!.Value, Does.Contain("Learning space not found"));
    }

    /// <summary>
    /// Presentation-004: Verify handler returns 400 Bad Request when whiteboard doesn't fit in learning space.
    /// </summary>
    [Test]
    [Description("Presentation-004: Return 400 Bad Request when whiteboard doesn't fit")]
    public async Task HandleAsync_WhiteboardDoesNotFit_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateWhiteboardDto(
            "IF-0103", "WB-001", 50.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ValidationException("Whiteboard does not fit in learning space"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>())
;        var badRequestResult = result.Result as BadRequest<string>;
        Assert.That(badRequestResult!.Value, Does.Contain("does not fit"));
    }

    /// <summary>
    /// Presentation-005: Verify handler returns 500 Internal Server Error when unexpected exception occurs.
    /// </summary>
    [Test]
    [Description("Presentation-005: Return 500 Internal Server Error when unexpected exception occurs")]
    public async Task HandleAsync_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateWhiteboardDto(
            "IF-0103", "WB-001", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", "Blue");

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>())
;        var problemResult = result.Result as ProblemHttpResult;
        Assert.That(problemResult!.ProblemDetails.Status, Is.EqualTo(500));
    }
}
