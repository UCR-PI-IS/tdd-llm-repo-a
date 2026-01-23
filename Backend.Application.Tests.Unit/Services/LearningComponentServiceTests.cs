using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for LearningComponentService
/// User Story: CPD-LC-001-001 - List components in a learning space
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
    }

    [Test]
    [Description("Service should return list of components when learning space has one or more components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponentsInLearningSpace_ShouldReturnComponentsList()
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

        var service = new LearningComponentService(_mockRepository.Object);

        // Act
        var result = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    [Description("Service should return empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponentsInLearningSpace_ShouldReturnEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        var emptyComponents = new List<LearningComponent>();

        _mockRepository.Setup(r => r.GetByLearningSpaceIdAsync(learningSpaceId))
                      .ReturnsAsync(emptyComponents);

        var service = new LearningComponentService(_mockRepository.Object);

        // Act
        var result = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    [Description("Service should throw ArgumentNullException when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullLearningSpaceId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new LearningComponentService(_mockRepository.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await service.GetComponentsByLearningSpaceIdAsync(null)
        );
    }

    [Test]
    [Description("Service should throw ArgumentException when learning space ID is empty string")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyLearningSpaceId_ShouldThrowArgumentException()
    {
        // Arrange
        var service = new LearningComponentService(_mockRepository.Object);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(
            async () => await service.GetComponentsByLearningSpaceIdAsync(string.Empty)
        );
    }

    [Test]
    [Description("Service should call repository GetByLearningSpaceIdAsync exactly once with correct learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_WhenCalled_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>();

        _mockRepository.Setup(r => r.GetByLearningSpaceIdAsync(learningSpaceId))
                      .ReturnsAsync(components);

        var service = new LearningComponentService(_mockRepository.Object);

        // Act
        await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        _mockRepository.Verify(r => r.GetByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }
}
