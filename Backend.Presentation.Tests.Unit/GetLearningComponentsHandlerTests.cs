using System.Collections.Generic;
using System.Threading.Tasks;
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

    #region HandleAsync Tests

    /// <summary>
    /// Verifies handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verifies handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_WithComponents_ReturnsOkResponseWithComponents()
    {
        // Arrange
        var learningSpaceId = "ls-001";
        var components = new List<LearningComponent>
        {
            new("comp-001", learningSpaceId, 2.5f, 3.0f, 2.0f, 10.0f, 5.0f, 0.0f, "North"),
            new("comp-002", learningSpaceId, 1.5f, 2.0f, 1.5f, 15.0f, 8.0f, 0.0f, "South")
        };

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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

        _mockService.Verify(service => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verifies handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_WithNoComponents_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        var learningSpaceId = "ls-empty";
        var emptyComponents = new List<LearningComponent>();

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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

        _mockService.Verify(service => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Empty learning space ID")]
    [TestCase("   ", Description = "Whitespace learning space ID")]
    [Description("Verifies handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidLearningSpaceId_ReturnsBadRequest(string invalidLearningSpaceId)
    {
        // Arrange
        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
            .ThrowsAsync(new System.ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

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
    [Description("Verifies handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = "ls-nonexistent";

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{nonExistentLearningSpaceId}' not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
    }

    #endregion
}
