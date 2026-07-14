using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> presentation handler.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    // ---------- Positive tests ----------

    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components.")]
    public async Task HandleAsync_SpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, Orientation.North),
            new LearningComponent("comp-002", learningSpaceId, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 0.0f, Orientation.South)
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
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value!.Components.Count, Is.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components.")]
    public async Task HandleAsync_SpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";
        var emptyComponents = new List<LearningComponent>();

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value, Is.Not.Null);
            Assert.That(okResult.Value!.Components.Count, Is.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
    }

    // ---------- Negative tests ----------

    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty.")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

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

    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist.")]
    public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space '{nonExistentLearningSpaceId}' not found."));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
    }
}
