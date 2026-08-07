using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the GetLearningComponentsHandler.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    /// <summary>
    /// Sets up the test context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Verifies handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_WithComponents_ReturnsOkResponse()
    {
        // Arrange
        String learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 3.0f, 2.0f, 1.5f, 5.0f, 0.0f, 2.0f, "East")
        };

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
        _mockService.Verify(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_WithNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        String learningSpaceId = "LS-002";
        var components = new List<LearningComponent>();

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
        _mockService.Verify(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        String invalidLearningSpaceId = "";

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        _mockService.Verify(s => s.GetComponentsByLearningSpaceIdAsync(It.IsAny<String>()), Times.Never);
    }

    /// <summary>
    /// Verifies handler returns NotFound response when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        String nonExistentLearningSpaceId = "LS-NOTEXIST";
        
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{nonExistentLearningSpaceId}' was not found."));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
    }
}
