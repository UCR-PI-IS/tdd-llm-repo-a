using Moq;
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
    private LearningComponentService _service = null!;

    /// <summary>
    /// Sets up the test context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    /// <summary>
    /// Verifies service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsList()
    {
        // Arrange
        String learningSpaceId = "LS-001";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent(
                "COMP-002", learningSpaceId, 3.0f, 2.0f, 1.5f, 5.0f, 0.0f, 2.0f, "East")
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
    /// Verifies service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        String learningSpaceId = "LS-002";
        var expectedComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(expectedComponents);

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
    /// Verifies service throws exception when learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is empty")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        String invalidLearningSpaceId = "";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));
        
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(It.IsAny<String>()), Times.Never);
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        String? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => 
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));
        
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(It.IsAny<String>()), Times.Never);
    }
}
