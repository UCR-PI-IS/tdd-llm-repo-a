using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the LearningComponentService.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository;
    private LearningComponentService _service;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync returns a list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithExistingComponents_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "IF-0103";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "component-001",
                learningSpaceId,
                2.5f,
                1.5f,
                1.0f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "component-002",
                learningSpaceId,
                3.0f,
                2.0f,
                1.5f,
                15.0f,
                10.0f,
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
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "IF-0202";
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
    /// Tests that GetComponentsByLearningSpaceIdAsync throws ArgumentException when learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = "";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync throws ArgumentException when learning space ID is null.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
