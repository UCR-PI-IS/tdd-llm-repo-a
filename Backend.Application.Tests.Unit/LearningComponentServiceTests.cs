using Moq;
using NUnit.Framework;
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
    private string _learningSpaceId = null!;

    /// <summary>
    /// Sets up test fixtures before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
        _learningSpaceId = "LS-001";
    }

    /// <summary>
    /// Verifies service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsListOfComponents()
    {
        // Arrange
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", _learningSpaceId, 2.5f, 1.8f, 0.5f, 1.0f, 0.0f, 2.0f, "North"),
            new LearningComponent("COMP-002", _learningSpaceId, 1.5f, 1.5f, 0.4f, 3.0f, 0.0f, 2.0f, "South")
        };

        _mockRepository
            .Setup(repo => repo.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(expectedComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].LearningSpaceId, Is.EqualTo(_learningSpaceId));
        Assert.That(result[1].LearningSpaceId, Is.EqualTo(_learningSpaceId));
    }

    /// <summary>
    /// Verifies service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var emptyList = new List<LearningComponent>();

        _mockRepository
            .Setup(repo => repo.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Verifies service throws exception when learning space ID is null or empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_InvalidLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

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
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
