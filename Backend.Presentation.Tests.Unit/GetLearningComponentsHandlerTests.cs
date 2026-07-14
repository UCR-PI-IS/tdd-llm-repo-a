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
    private Mock<ILearningComponentService> _mockService;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Verifies handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Returns OK with list of components when learning space has components")]
    public async Task HandleAsync_WithComponents_ReturnsOkWithList()
    {
        // Arrange
        string learningSpaceId = "SPACE-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.5f, 0.0f, 1.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1.0f, 1.0f, 1.0f, 2.0f, 0.0f, 1.0f, "South")
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
    }

    /// <summary>
    /// Verifies handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Returns OK with empty list when learning space has no components")]
    public async Task HandleAsync_WithNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        string learningSpaceId = "SPACE-002";
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
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Returns BadRequest when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

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
    [Description("Returns NotFound when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        string nonExistentLearningSpaceId = "SPACE-999";

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
