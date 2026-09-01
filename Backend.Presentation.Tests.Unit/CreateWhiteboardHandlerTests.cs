using Microsoft.AspNetCore.Http;
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
    /// Presentation-001: Verify that the handler successfully creates a whiteboard
    /// and returns 200 OK with the created resource.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with the created whiteboard resource")]
    public async Task HandleAsync_ValidInput_ReturnsOk()
    {
        // Arrange
        var createdWhiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);
        
        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ReturnsAsync(createdWhiteboard);

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreateWhiteboardResponse>>());
            var okResult = result.Result as Ok<CreateWhiteboardResponse>;
            Assert.That(okResult!.Value.Whiteboard.MarkerColor, Is.EqualTo(ValidMarkerColor));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that the handler returns 400 Bad Request when
    /// input validation fails (invalid orientation).
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 Bad Request for invalid orientation")]
    public async Task HandleAsync_InvalidOrientation_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "InvalidOrientation", ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ArgumentException("Invalid orientation"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("Invalid orientation"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that the handler returns 404 Not Found when
    /// learning space doesn't exist.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 404 Not Found when learning space doesn't exist")]
    public async Task HandleAsync_LearningSpaceNotFound_ReturnsNotFound()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            ValidComponentId, "NON-EXISTENT", 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new NotFoundException("Learning space not found"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
            var notFoundResult = result.Result as NotFound<string>;
            Assert.That(notFoundResult!.Value, Does.Contain("Learning space not found"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify that the handler returns 400 Bad Request when
    /// whiteboard doesn't fit in learning space.
    /// </summary>
    [Test]
    [Description("Presentation-004: Handler returns 400 Bad Request when whiteboard doesn't fit")]
    public async Task HandleAsync_WhiteboardDoesNotFit_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId, 50.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new ValidationException("Whiteboard does not fit in learning space"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("does not fit"));
        });
    }

    /// <summary>
    /// Presentation-005: Verify that the handler returns 500 Internal Server Error
    /// when unexpected exception occurs.
    /// </summary>
    [Test]
    [Description("Presentation-005: Handler returns 500 Internal Server Error for unexpected exceptions")]
    public async Task HandleAsync_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateWhiteboardRequest(
            ValidComponentId, ValidLearningSpaceId, 5.0f, 2.0f, 3.0f, 0f, 0f, 0f, "North", ValidMarkerColor);

        _mockService
            .Setup(s => s.CreateWhiteboardAsync(It.IsAny<CreateWhiteboardRequest>()))
            .ThrowsAsync(new InvalidOperationException("Unexpected error"));

        // Act
        var result = await CreateWhiteboardHandler.HandleAsync(_mockService.Object, request);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<ProblemHttpResult>());
        var problemResult = result.Result as ProblemHttpResult;
        Assert.That(problemResult!.ProblemDetails.Status, Is.EqualTo(500));
    }
}

/// <summary>
/// Request object for creating a whiteboard.
/// </summary>
public record CreateWhiteboardRequest(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Response object for creating a whiteboard.
/// </summary>
public record CreateWhiteboardResponse(Whiteboard Whiteboard);

/// <summary>
/// Static handler class for creating whiteboards.
/// </summary>
public static class CreateWhiteboardHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new whiteboard.
    /// </summary>
    /// <param name="service">The whiteboard service.</param>
    /// <param name="request">The request containing the creation parameters.</param>
    /// <returns>
    /// A <see cref="Ok{T}"/> response with the created whiteboard,
    /// a <see cref="BadRequest{T}"/> if validation fails,
    /// a <see cref="NotFound{T}"/> if learning space doesn't exist,
    /// or a <see cref="ProblemHttpResult"/> if an unexpected error occurs.
    /// </returns>
    public static async Task<Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult>> HandleAsync(
        IWhiteboardService service,
        CreateWhiteboardRequest request)
    {
        try
        {
            var whiteboard = await service.CreateWhiteboardAsync(request);
            return TypedResults.Ok(new CreateWhiteboardResponse(whiteboard));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return TypedResults.NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred.", statusCode: 500);
        }
    }
}

/// <summary>
/// Interface for the whiteboard service.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>The created whiteboard.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}
