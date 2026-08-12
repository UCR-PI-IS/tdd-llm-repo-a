using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers;

/// <summary>
/// Unit tests for <see cref="GetLearningComponentsHandler"/>.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    private const string ValidLearningSpaceId = "LS-001";
    private const string NonExistentLearningSpaceId = "LS-999";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.Reset();
    }

    /// <summary>
    /// Presentation-001: Verifies the handler returns an OK response with a list
    /// of components when the learning space has components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, "South")
        };

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(
            _mockService.Object, learningSpaceId);

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
    /// Presentation-002: Verifies the handler returns an OK response with an empty
    /// list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var emptyComponents = new List<LearningComponent>();

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(
            _mockService.Object, learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components, Is.Empty);
        });
    }

    /// <summary>
    /// Presentation-003: Verifies the handler returns a BadRequest response
    /// when the learning space ID is null or empty.
    /// </summary>
    [Test]
    [TestCase("", Description = "Empty learning space ID returns BadRequest")]
    [TestCase(null!, Description = "Null learning space ID returns BadRequest")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest(string? invalidLearningSpaceId)
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException(
                "Learning space ID cannot be null or empty", "learningSpaceId"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(
            _mockService.Object, invalidLearningSpaceId!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = result as BadRequest<ErrorResponse>;
            Assert.That(badRequestResult!.Value!.Message,
                Does.Contain("Learning space ID cannot be null or empty"));
        });
    }

    /// <summary>
    /// Presentation-004: Verifies the handler returns a NotFound response
    /// when the learning space does not exist.
    /// </summary>
    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_LearningSpaceDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = NonExistentLearningSpaceId;

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException(
                $"Learning space with ID '{nonExistentLearningSpaceId}' was not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(
            _mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
            var notFoundResult = result as NotFound<ErrorResponse>;
            Assert.That(notFoundResult!.Value!.Message,
                Does.Contain(nonExistentLearningSpaceId));
        });
    }
}
