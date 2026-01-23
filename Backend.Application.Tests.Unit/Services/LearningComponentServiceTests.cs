using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the LearningComponentService.
/// Tests verify service logic, validation, and repository delegation.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository;
    private ILearningComponentService _service;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    [Test]
    [Description("Service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsComponentsList()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(1, learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North"),
            new LearningComponent(2, learningSpaceId, 3.0f, 2.0f, 0.3f, 2.0f, 1.0f, 4.0f, "South")
        };

        _mockRepository.Setup(r => r.GetByLearningSpaceIdAsync(learningSpaceId))
                       .ReturnsAsync(components);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    [Description("Service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        var emptyComponents = new List<LearningComponent>();

        _mockRepository.Setup(r => r.GetByLearningSpaceIdAsync(learningSpaceId))
                       .ReturnsAsync(emptyComponents);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    [Description("Service throws ArgumentNullException when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentNullException()
    {
        // Arrange
        string nullLearningSpaceId = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId)
        );
    }

    [Test]
    [Description("Service throws ArgumentException when learning space ID is empty string")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        var emptyLearningSpaceId = string.Empty;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(emptyLearningSpaceId)
        );
    }

    [Test]
    [Description("Service calls repository with correct learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_CallsRepositoryOnce()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>();

        _mockRepository.Setup(r => r.GetByLearningSpaceIdAsync(learningSpaceId))
                       .ReturnsAsync(components);

        // Act
        await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        _mockRepository.Verify(r => r.GetByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }
}
