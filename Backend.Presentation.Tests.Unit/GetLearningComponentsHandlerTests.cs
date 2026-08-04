using Moq;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> class.
/// Tests the HandleAsync method with a mocked service for all response scenarios.
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

    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components (Presentation-001)")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2f, 3f, 1f, 5f, 10f, 0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1f, 2f, 0.5f, 8f, 12f, 0f, "South")
        };
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components, Has.Count.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components (Presentation-002)")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components, Is.Empty);
        });
    }

    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty (Presentation-003)")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest()
    {
        // Arrange
        var invalidLearningSpaceId = "";
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
            .ThrowsAsync(new ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = result as BadRequest<ErrorResponse>;
            Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        });
    }

    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist (Presentation-004)")]
    public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = "NON-EXISTENT";
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space '{nonExistentLearningSpaceId}' not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
            var notFoundResult = result as NotFound<ErrorResponse>;
            Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
        });
    }
}
