using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> application service.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private LearningComponentService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    // ---------- Positive tests ----------

    [Test]
    [Description("Verify service returns list of components when learning space has components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_SpaceHasComponents_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, Orientation.North),
            new LearningComponent("comp-002", learningSpaceId, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 0.0f, Orientation.South)
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
    }

    [Test]
    [Description("Verify service returns empty list when learning space has no components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_SpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";
        var emptyList = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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

    // ---------- Negative tests ----------

    [Test]
    [Description("Verify service throws exception when learning space ID is empty.")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null.")]
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
