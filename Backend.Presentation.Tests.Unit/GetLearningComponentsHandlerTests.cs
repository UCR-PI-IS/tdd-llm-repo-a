using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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

    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Verifies handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_WithExistingComponents_ReturnsOkWithComponentList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001", learningSpaceId, 2.0f, 3.0f, 1.0f, 10.0f, 5.0f, 0.0f, "North"),
            new LearningComponent(
                "comp-002", learningSpaceId, 1.5f, 2.5f, 1.0f, 15.0f, 8.0f, 0.0f, "South")
        };

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
        Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
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
        string learningSpaceId = "space-empty";
        var emptyComponents = new List<LearningComponent>();

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
        Assert.That(okResult.Value.Components, Is.Empty);
    }

    /// <summary>
    /// Verifies handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidLearningSpaceId_ReturnsBadRequest()
    {
        // Arrange
        string invalidLearningSpaceId = "";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
            .ThrowsAsync(new ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
    }

    /// <summary>
    /// Verifies handler returns NotFound response when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space '{nonExistentLearningSpaceId}' not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
    }
}
