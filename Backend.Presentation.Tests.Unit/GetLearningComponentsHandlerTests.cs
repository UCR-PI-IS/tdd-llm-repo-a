using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="GetLearningComponentsHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-004.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";
    private const string NonExistentLearningSpaceId = "NON-EXISTENT-999";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify handler returns OK response with list of components
    /// when learning space has components.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify handler returns OK response with list of components")]
    public async Task HandleAsync_ValidIdWithComponents_ReturnsOk()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 3.0f, 2.0f, 1.0f, 15f, 25f, 0f, "South")
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

    /// <summary>
    /// Presentation-002: Verify handler returns OK response with empty list
    /// when learning space has no components.
    /// </summary>
    [Test]
    [Description("Presentation-002: Verify handler returns OK response with empty list")]
    public async Task HandleAsync_ValidIdWithNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
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
            Assert.That(okResult!.Value!.Components, Is.Empty);
            Assert.That(okResult.Value.Components, Has.Count.EqualTo(0));
        });
    }

    /// <summary>
    /// Presentation-003: Verify handler returns BadRequest response when
    /// learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Presentation-003: Empty string learning space ID returns BadRequest")]
    [TestCase(null, Description = "Presentation-003: Null learning space ID returns BadRequest")]
    public async Task HandleAsync_InvalidId_ReturnsBadRequest(string? invalidLearningSpaceId)
    {
        // Arrange
        // No service setup needed — handler validates input before calling service

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = result as BadRequest<ErrorResponse>;
            Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        });
    }

    /// <summary>
    /// Presentation-004: Verify handler returns NotFound response when
    /// learning space does not exist.
    /// </summary>
    [Test]
    [Description("Presentation-004: Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentSpace_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = NonExistentLearningSpaceId;

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
