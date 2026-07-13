using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers;

/// <summary>
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> class.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    /// <summary>
    /// Sets up the test context with a mocked service.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Verifies that the handler returns an OK response with a list of components when the learning space has components.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify handler returns OK response with list of components")]
    public async Task HandleAsync_WithComponents_ReturnsOkWithList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.0f, 1.0f, 2.0f, 0.0f, 0.0f, "South")
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
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    /// <summary>
    /// Verifies that the handler returns an OK response with an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Presentation-002: Verify handler returns OK response with empty list")]
    public async Task HandleAsync_WithNoComponents_ReturnsOkWithEmptyList()
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies that the handler returns a BadRequest response when the learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Presentation-003: Verify handler returns BadRequest response when learning space ID is invalid")]
    public async Task HandleAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

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

    /// <summary>
    /// Verifies that the handler returns a NotFound response when the learning space does not exist.
    /// </summary>
    [Test]
    [Description("Presentation-004: Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentSpace_ReturnsNotFound()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{nonExistentLearningSpaceId}' not found"));

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
