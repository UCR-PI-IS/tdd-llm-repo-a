using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> class.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private ILearningComponentService _service = null!;

    /// <summary>
    /// Sets up the test fixtures before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies that the service returns a list of components when the learning space has components.
    /// </summary>
    [Test]
    [Description("Service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsComponentList()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                learningSpaceId,
                2.5f,
                3.0f,
                1.5f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "comp-002",
                learningSpaceId,
                3.0f,
                2.5f,
                2.0f,
                15.0f,
                8.0f,
                0.0f,
                "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(expectedComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies that the service returns an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "space-002";
        var emptyComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Verifies that the service throws an exception when the learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Service throws ArgumentException when learning space ID is empty")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        var invalidLearningSpaceId = string.Empty;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Verifies that the service throws an exception when the learning space ID is null.
    /// </summary>
    [Test]
    [Description("Service throws ArgumentException when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
