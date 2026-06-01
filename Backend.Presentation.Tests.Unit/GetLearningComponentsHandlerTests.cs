using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="GetLearningComponentsHandler"/> class.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    private const string ValidLearningSpaceId = "LS-001";
    private const string EmptyLearningSpaceId = "";
    private const string NonExistentLearningSpaceId = "LS-999";

    /// <summary>
    /// Sets up the mock service before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    /// <summary>
    /// Creates a list of LearningComponent entities for testing.
    /// </summary>
    private static List<LearningComponent> CreateTestComponents(string learningSpaceId, int count)
    {
        var components = new List<LearningComponent>();
        for (int i = 1; i <= count; i++)
        {
            components.Add(new LearningComponent(
                $"COMP-{i:D3}",
                learningSpaceId,
                10f + i,
                5f + i,
                8f + i,
                2f + i,
                3f + i,
                1f + i,
                "North"));
        }
        return components;
    }

    #region Positive Tests

    /// <summary>
    /// Verify handler returns OK response with list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_LearningSpaceHasComponents_ShouldReturnOkWithComponentList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var testComponents = CreateTestComponents(learningSpaceId, 2);

        _mockService
            .Setup(svc => svc.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(testComponents);

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
    /// Verify handler returns OK response with empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var testComponents = new List<LearningComponent>();

        _mockService
            .Setup(svc => svc.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(testComponents);

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

    #endregion

    #region Negative Tests

    /// <summary>
    /// Verify handler returns BadRequest response when learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is empty")]
    public async Task HandleAsync_EmptyLearningSpaceId_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidLearningSpaceId = EmptyLearningSpaceId;

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
    /// Verify handler returns NotFound response when learning space does not exist.
    /// </summary>
    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentLearningSpaceId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentLearningSpaceId = NonExistentLearningSpaceId;

        _mockService
            .Setup(svc => svc.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

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

    #endregion
}