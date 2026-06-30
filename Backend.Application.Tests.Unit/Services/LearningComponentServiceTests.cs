using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> application service.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository;
    private LearningComponentService _service;
    private string _learningSpaceId;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
        _learningSpaceId = "space-001";
    }

    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", _learningSpaceId, 10.0f, 5.0f, 8.0f, 1.0f, 2.0f, 3.0f, "North"),
            new LearningComponent("comp-002", _learningSpaceId, 6.0f, 4.0f, 7.0f, 4.0f, 5.0f, 6.0f, "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
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
    }

    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(_learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        var invalidLearningSpaceId = string.Empty;

        // Act & Assert
        Assert.That(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("learningSpaceId"));
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act & Assert
        Assert.That(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("learningSpaceId"));
    }
}
