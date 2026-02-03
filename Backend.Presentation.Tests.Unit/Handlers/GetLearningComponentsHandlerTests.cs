using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers;

/// <summary>
/// Unit tests for GetLearningComponentsHandler
/// User Story: CPD-LC-001-001 - List components in a learning space
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

    [Test]
    [Description("Handler should return OK with components when learning space has components")]
    public async Task HandleAsync_WithComponentsInLearningSpace_ShouldReturnOkWithComponents()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(1, learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North")
        };

        _mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                   .ReturnsAsync(components);

        var handler = new GetLearningComponentsHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(learningSpaceId);

        // Assert
        Assert.That(result.Result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
    }

    [Test]
    [Description("Handler should return OK with empty list when learning space has no components")]
    public async Task HandleAsync_WithNoComponentsInLearningSpace_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        var emptyComponents = new List<LearningComponent>();

        _mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                   .ReturnsAsync(emptyComponents);

        var handler = new GetLearningComponentsHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(learningSpaceId);

        // Assert
        Assert.That(result.Result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
    }

    [Test]
    [TestCase(null, Description = "Handler should return BadRequest when learning space ID is null")]
    [TestCase("", Description = "Handler should return BadRequest when learning space ID is empty string")]
    public async Task HandleAsync_WithInvalidLearningSpaceId_ShouldReturnBadRequest(string invalidLearningSpaceId)
    {
        // Arrange
        var handler = new GetLearningComponentsHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(invalidLearningSpaceId);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
    }

    [Test]
    [Description("Handler should correctly map components to DTOs in response")]
    public async Task HandleAsync_WithComponents_ShouldMapComponentsToDtosCorrectly()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(1, learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North")
        };

        _mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                   .ReturnsAsync(components);

        var handler = new GetLearningComponentsHandler(_mockService.Object);

        // Act
        var response = await handler.HandleAsync(learningSpaceId);
        var result = response.Result as Ok<GetLearningComponentsResponse>;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Value, Is.Not.Null);
        Assert.That(result.Value.Components.Count, Is.EqualTo(1));
        Assert.That(result.Value.Components[0].ComponentId, Is.EqualTo(1));
    }

    [Test]
    [Description("Handler should call service with correct learning space ID")]
    public async Task HandleAsync_WhenCalled_ShouldCallServiceWithCorrectId()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        _mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
                   .ReturnsAsync(new List<LearningComponent>());

        var handler = new GetLearningComponentsHandler(_mockService.Object);

        // Act
        await handler.HandleAsync(learningSpaceId);

        // Assert
        _mockService.Verify(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }
}
