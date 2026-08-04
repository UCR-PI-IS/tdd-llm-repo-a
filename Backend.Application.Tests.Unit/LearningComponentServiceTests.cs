using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> class.
/// Tests the GetComponentsByLearningSpaceIdAsync method with mocked repository.
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

    [Test]
    [Description("Verify service returns list of components when learning space has components (Application-001)")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2f, 3f, 1f, 5f, 10f, 0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1f, 2f, 0.5f, 8f, 12f, 0f, "South")
        };
        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    [Test]
    [Description("Verify service returns empty list when learning space has no components (Application-002)")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [TestCase("", Description = "Verify service throws ArgumentException when learning space ID is empty (Application-003)")]
    [TestCase(" ", Description = "Verify service throws ArgumentException when learning space ID is whitespace (Application-003)")]
    public async Task GetComponentsByLearningSpaceIdAsync_EmptyOrWhitespaceLearningSpaceId_ThrowsArgumentException(
        string invalidLearningSpaceId)
    {
        // Act & Assert
        Assert.That(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("learningSpaceId"));
    }

    [Test]
    [Description("Verify service throws ArgumentException when learning space ID is null (Application-004)")]
    public async Task GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.That(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(null!),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("learningSpaceId"));
    }
}
