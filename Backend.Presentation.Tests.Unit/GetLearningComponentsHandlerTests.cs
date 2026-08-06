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
    private Mock<ILearningComponentService> _mockService;
    private string _learningSpaceId;
    private string _nonExistentLearningSpaceId;
    private List<LearningComponent> _components;
    private List<LearningComponent> _emptyComponents;

    /// <summary>
    /// Sets up the test fixture with mocks and test data.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
        _learningSpaceId = "SPACE-001";
        _nonExistentLearningSpaceId = "NON-EXISTENT";
        
        _components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", _learningSpaceId, 2.0f, 1.5f, 1.0f, 10.0f, 5.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", _learningSpaceId, 1.5f, 1.0f, 0.8f, 15.0f, 8.0f, 0.0f, "South")
        };
        
        _emptyComponents = new List<LearningComponent>();
    }

    /// <summary>
    /// Tests that handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_WithComponents_ReturnsOkWithComponents()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(_components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, _learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>(), "Expected Ok<GetLearningComponentsResponse> result type");
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.That(okResult!.Value, Is.Not.Null, "Expected non-null response value");
        Assert.That(okResult.Value!.Components.Count, Is.EqualTo(2), "Expected 2 components in response");
        Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(_learningSpaceId), "Expected first component to have matching LearningSpaceId");
    }

    /// <summary>
    /// Tests that handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_NoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(_emptyComponents);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, _learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>(), "Expected Ok<GetLearningComponentsResponse> result type");
        var okResult = result as Ok<GetLearningComponentsResponse>;
        Assert.That(okResult!.Value, Is.Not.Null, "Expected non-null response value");
        Assert.That(okResult.Value!.Components.Count, Is.EqualTo(0), "Expected 0 components in response");
        Assert.That(okResult.Value.Components, Is.Empty, "Expected empty components list");
    }

    /// <summary>
    /// Tests that handler returns BadRequest response when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        string invalidLearningSpaceId = "";
        
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId))
            .ThrowsAsync(new ArgumentException("Learning space ID cannot be null or empty", "learningSpaceId"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>(), "Expected BadRequest<ErrorResponse> result type");
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.That(badRequestResult!.Value, Is.Not.Null, "Expected non-null error response value");
        Assert.That(badRequestResult.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"), "Expected error message about invalid learning space ID");
    }

    /// <summary>
    /// Tests that handler returns NotFound response when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentSpace_ReturnsNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space with ID '{_nonExistentLearningSpaceId}' not found"));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, _nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>(), "Expected NotFound<ErrorResponse> result type");
        var notFoundResult = result as NotFound<ErrorResponse>;
        Assert.That(notFoundResult!.Value, Is.Not.Null, "Expected non-null error response value");
        Assert.That(notFoundResult.Value!.Message, Does.Contain(_nonExistentLearningSpaceId), "Expected error message to contain the non-existent learning space ID");
    }
}
