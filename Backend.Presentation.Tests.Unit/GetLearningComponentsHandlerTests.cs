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
    private Mock<ILearningComponentService> _mockService = null!;

    /// <summary>
    /// Sets up mocks before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Creates a test LearningComponent with specified parameters.
    /// </summary>
    private LearningComponent CreateTestComponent(String componentId, String learningSpaceId)
    {
        return new LearningComponent(
            componentId,
            learningSpaceId,
            10.0f,  // width
            5.0f,   // height
            8.0f,   // depth
            1.0f,   // x
            2.0f,   // y
            3.0f,   // z
            "North");
    }

    /// <summary>
    /// Verifies handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_WithComponents_ReturnsOkResponse()
    {
        // Arrange
        String learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            CreateTestComponent("component-001", learningSpaceId),
            CreateTestComponent("component-002", learningSpaceId)
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
    }

    /// <summary>
    /// Verifies handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Presentation-002: Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_WithNoComponents_ReturnsOkResponseWithEmptyList()
    {
        // Arrange
        String learningSpaceId = "space-002";
        var emptyList = new List<LearningComponent>();

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyList);

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
    /// Verifies handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Presentation-003: Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidId_ReturnsBadRequestResponse()
    {
        // Arrange
        String invalidLearningSpaceId = "";

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
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
    /// Verifies handler returns NotFound response when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Presentation-004: Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_WithNonExistentSpace_ReturnsNotFoundResponse()
    {
        // Arrange
        String nonExistentLearningSpaceId = "space-nonexistent";

        _mockService
            .Setup(service => service.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
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
