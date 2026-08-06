using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the LearningComponentService.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private ILearningComponentService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithExistingComponents_ReturnsComponentList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001", learningSpaceId, 2.0f, 3.0f, 1.0f, 10.0f, 5.0f, 0.0f, "North"),
            new LearningComponent(
                "comp-002", learningSpaceId, 1.5f, 2.5f, 1.0f, 15.0f, 8.0f, 0.0f, "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
        Assert.That(result[1].LearningSpaceId, Is.EqualTo(learningSpaceId));
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-empty";
        var emptyComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = "";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
