using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers;

/// <summary>
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> presentation handler.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService;
    private string _learningSpaceId;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
        _learningSpaceId = "space-001";
    }

    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", _learningSpaceId, 10.0f, 5.0f, 8.0f, 1.0f, 2.0f, 3.0f, "North"),
            new LearningComponent("comp-002", _learningSpaceId, 6.0f, 4.0f, 7.0f, 4.0f, 5.0f, 6.0f, "South")
        };

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, _learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = (Ok<GetLearningComponentsResponse>)result;
            Assert.That(okResult.Value!.Components.Count, Is.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(_learningSpaceId));
        });
    }

    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, _learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = (Ok<GetLearningComponentsResponse>)result;
            Assert.That(okResult.Value!.Components.Count, Is.EqualTo(0));
            Assert.That(okResult.Value.Components, Is.Empty);
        });
    }

    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest()
    {
        // Arrange
        var invalidLearningSpaceId = string.Empty;

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = (BadRequest<ErrorResponse>)result;
            Assert.That(badRequestResult.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        });
    }

    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentLearningSpaceId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = "space-999";

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space '{nonExistentLearningSpaceId}' not found."));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
            var notFoundResult = (NotFound<ErrorResponse>)result;
            Assert.That(notFoundResult.Value!.Message, Does.Contain(nonExistentLearningSpaceId));
        });
    }
}
