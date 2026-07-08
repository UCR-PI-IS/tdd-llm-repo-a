using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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

    /// <summary>
    /// Sets up mocks and SUT before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
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
    /// Verifies service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsListOfComponents()
    {
        // Arrange
        String learningSpaceId = "space-001";
        var expectedComponents = new List<LearningComponent>
        {
            CreateTestComponent("component-001", learningSpaceId),
            CreateTestComponent("component-002", learningSpaceId)
        };

        _mockRepository
            .Setup(repo => repo.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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
    }

    /// <summary>
    /// Verifies service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        String learningSpaceId = "space-002";
        var emptyList = new List<LearningComponent>();

        _mockRepository
            .Setup(repo => repo.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Application-003: Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        String invalidLearningSpaceId = "";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));
        
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null.
    /// </summary>
    [Test]
    [Description("Application-004: Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        String? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));
        
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
