using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the LearningComponentService.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private ILearningComponentService _service = null!;
    private string _learningSpaceId = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
        _learningSpaceId = "space-001";
    }

    /// <summary>
    /// Test that service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsComponentList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001", _learningSpaceId, 10f, 5f, 8f, 1f, 2f, 3f, Orientation.North),
            new LearningComponent(
                "comp-002", _learningSpaceId, 8f, 4f, 6f, 4f, 5f, 6f, Orientation.South)
        };
        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(_learningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(_learningSpaceId));
        });
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Test that service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var emptyComponents = new List<LearningComponent>();
        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId), Times.Once);
    }

    /// <summary>
    /// Test that service throws exception when learning space ID is null or empty.
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
    /// Test that service throws exception when learning space ID is null.
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
