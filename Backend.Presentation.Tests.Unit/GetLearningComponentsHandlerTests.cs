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
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> class.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    /// <summary>
    /// Sets up the test fixtures before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Verifies that the handler returns an OK response with a list of components
    /// when the learning space has components.
    /// </summary>
    [Test]
    [Description("Handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                learningSpaceId,
                2.5f,
                3.0f,
                1.5f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "comp-002",
                learningSpaceId,
                3.0f,
                2.5f,
                2.0f,
                15.0f,
                8.0f,
                0.0f,
                "South")
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
    }

    /// <summary>
    /// Verifies that the handler returns an OK response with an empty list
    /// when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = "space-002";
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
    /// Verifies that the handler returns a BadRequest response when the learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest()
    {
        // Arrange
        var invalidLearningSpaceId = string.Empty;

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
    }

    /// <summary>
    /// Verifies that the handler returns a NotFound response when the learning space does not exist.
    /// </summary>
    [Test]
    [Description("Handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentLearningSpaceId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = "non-existent-space";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space {nonExistentLearningSpaceId} not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
    }
}
