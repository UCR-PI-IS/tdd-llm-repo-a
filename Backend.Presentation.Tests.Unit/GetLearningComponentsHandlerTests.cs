using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="GetLearningComponentsHandler"/>.
/// Verifies the handler returns the correct HTTP result type for each scenario.
/// </summary>
[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    private const string ValidLearningSpaceId = "IF-0103";
    private const string NonExistentLearningSpaceId = "NON-EXISTENT-ID";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService = null!;
    }

    // ────────────────────────────────────────────────────────────────────
    // Presentation-001  –  OK with components
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Presentation-001: Verify handler returns OK response with list of components when learning space has components.")]
    public async Task HandleAsync_LearningSpaceHasComponents_ReturnsOkWithComponents()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", ValidLearningSpaceId, 2f, 1.5f, 0.5f, 1f, 2f, 0f, "North"),
            new LearningComponent("COMP-002", ValidLearningSpaceId, 3f, 2f, 1f, 3f, 4f, 0f, "South")
        };

        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components.Count, Is.EqualTo(2));
            Assert.That(okResult.Value.Components[0].LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Presentation-002  –  OK with empty list
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Presentation-002: Verify handler returns OK response with empty list when learning space has no components.")]
    public async Task HandleAsync_LearningSpaceHasNoComponents_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
            var okResult = result as Ok<GetLearningComponentsResponse>;
            Assert.That(okResult!.Value!.Components, Is.Empty);
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Presentation-003  –  BadRequest for invalid learning space ID
    // ────────────────────────────────────────────────────────────────────

    [TestCase(null, Description = "Null learning space ID returns BadRequest")]
    [TestCase("", Description = "Empty learning space ID returns BadRequest")]
    [Description("Presentation-003: Verify handler returns BadRequest response when learning space ID is null or empty.")]
    public async Task HandleAsync_InvalidLearningSpaceId_ReturnsBadRequest(string? invalidLearningSpaceId)
    {
        // Arrange & Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, invalidLearningSpaceId!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
            var badRequestResult = result as BadRequest<ErrorResponse>;
            Assert.That(badRequestResult!.Value!.Message, Does.Contain("Learning space ID cannot be null or empty"));
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Presentation-004  –  NotFound for non-existent learning space
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Presentation-004: Verify handler returns NotFound response when learning space does not exist.")]
    public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange — service throws KeyNotFoundException for non-existent spaces
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(NonExistentLearningSpaceId))
            .ThrowsAsync(new KeyNotFoundException($"Learning space '{NonExistentLearningSpaceId}' not found."));

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(_mockService.Object, NonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
            var notFoundResult = result as NotFound<ErrorResponse>;
            Assert.That(notFoundResult!.Value!.Message, Does.Contain(NonExistentLearningSpaceId));
        });
    }
}
