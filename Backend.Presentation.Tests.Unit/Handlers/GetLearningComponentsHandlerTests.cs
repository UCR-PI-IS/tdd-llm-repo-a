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
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> class.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentListService> _mockService = null!;

    private const string ValidLearningSpaceId = "IF-0103";
    private const string NonExistentLearningSpaceId = "NON-EXISTENT";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentListService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService = null!;
    }

    /// <summary>
    /// Verifies the handler returns an OK response with a list of components
    /// when the learning space has components.
    /// </summary>
    [Test(Description = "Presentation-001: Returns OK with components when learning space has components")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 2.0f, 0.0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 1.0f, 1.0f, 0.3f, 3.0f, 4.0f, 0.0f, "South")
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
            Assert.That(okResult!.Value!.Components, Has.Count.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    /// <summary>
    /// Verifies the handler returns an OK response with an empty list
    /// when the learning space has no components.
    /// </summary>
    [Test(Description = "Presentation-002: Returns OK with empty list when learning space has no components")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
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
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.Multiple(() =>
        {
            Assert.That(okResult!.Value!.Components, Has.Count.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies the handler returns a BadRequest response when the learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Presentation-003: Empty learning space ID returns BadRequest")]
    [TestCase(null, Description = "Presentation-003: Null learning space ID returns BadRequest")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest(string? invalidLearningSpaceId)
    {
        // Arrange — service should not be called for invalid input
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId!);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
    }

    /// <summary>
    /// Verifies the handler returns a NotFound response when the learning space does not exist.
    /// </summary>
    [Test(Description = "Presentation-004: Returns NotFound when learning space does not exist")]
    public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = NonExistentLearningSpaceId;

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{nonExistentId}' was not found."));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value!.Message, Does.Contain(nonExistentId));
    }
}
