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
    private Mock<IWhiteboardCreateService> _mockService = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.5f;
    private const float ValidZ = 1.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IWhiteboardCreateService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static CreateWhiteboardDto CreateValidDto()
    {
        return new CreateWhiteboardDto(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    /// <summary>
    /// Presentation-001: Verify that the handler returns 200 OK with the created whiteboard
    /// response when the request is valid and the service succeeds.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns Ok with created whiteboard when request is valid")]
    public async Task HandleAsync_ValidInput_ReturnsOkWithWhiteboard()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ReturnsAsync(whiteboard);

        var dto = CreateValidDto();

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreateWhiteboardResponse>>());
            var okResult = (Ok<CreateWhiteboardResponse>)result.Result;
            Assert.That(okResult.Value.Whiteboard.MarkerColor, Is.EqualTo(ValidMarkerColor));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that the handler returns 400 Bad Request when the service
    /// throws ArgumentException for invalid orientation.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns BadRequest when input validation fails (invalid orientation)")]
    public async Task HandleAsync_InvalidOrientation_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ArgumentException("Invalid orientation 'North' for whiteboard"));

        var dto = CreateValidDto();

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Invalid orientation"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that the handler returns 404 Not Found when the service
    /// throws NotFoundException because the learning space does not exist.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns NotFound when learning space does not exist")]
    public async Task HandleAsync_LearningSpaceNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new NotFoundException("Learning space not found"));

        var dto = CreateValidDto();

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
            var notFoundResult = (NotFound<string>)result.Result;
            Assert.That(notFoundResult.Value, Does.Contain("Learning space not found"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that the handler returns 400 Bad Request when the service
    /// throws ValidationException because the whiteboard does not fit in the learning space.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns BadRequest when whiteboard does not fit in learning space")]
    public async Task HandleAsync_WhiteboardDoesNotFit_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ValidationException("Whiteboard does not fit in the learning space"));

        var dto = CreateValidDto();

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("does not fit"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that the handler returns 500 Internal Server Error
    /// when the service throws an unexpected exception.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 500 Internal Server Error for unexpected exceptions")]
    public async Task HandleAsync_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected database failure"));

        var dto = CreateValidDto();

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>());
    }
}
