using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> class.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private LearningComponentService _service = null!;

    private const string ValidLearningSpaceId = "LS-001";
    private const string EmptyLearningSpaceId = "";
    private const string? NullLearningSpaceId = null;

    /// <summary>
    /// Sets up the mock repository and service instance before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    #region Positive Tests

    /// <summary>
    /// Verify service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ShouldReturnComponentList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 10f, 5f, 8f, 2f, 3f, 1f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 12f, 6f, 9f, 4f, 5f, 2f, "South")
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
    /// Verify service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ShouldReturnEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var expectedComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(repo => repo.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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
    }

    #endregion

    #region Negative Tests

    /// <summary>
    /// Verify service throws exception when learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is empty")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidLearningSpaceId = EmptyLearningSpaceId;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));
        Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Verify service throws exception when learning space ID is null.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ShouldThrowArgumentException()
    {
        // Arrange
        var nullLearningSpaceId = NullLearningSpaceId;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));
        Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
    }

    #endregion
}