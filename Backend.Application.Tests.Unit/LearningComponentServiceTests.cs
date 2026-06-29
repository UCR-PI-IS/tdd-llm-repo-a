using Moq;
using NUnit;
using NUnit.Framework;
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

    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_SpaceHasComponents_ReturnsComponentList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 10f, 5f, 8f, 1f, 2f, 3f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 6f, 4f, 7f, 2f, 3f, 4f, "South")
        };
        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

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
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_SpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

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

    [Test]
    [Description("Verify service throws exception when learning space ID is empty")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = string.Empty;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
        });
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
        });
    }
}
