using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> class.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentListRepository> _mockRepository = null!;
    private LearningComponentService _sut = null!;

    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentListRepository>();
        _sut = new LearningComponentService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository = null!;
        _sut = null!;
    }

    /// <summary>
    /// Verifies the service returns a list of components when the learning space has components.
    /// </summary>
    [Test(Description = "Application-001: Returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsComponentList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 2.0f, 0.0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 1.0f, 1.0f, 0.3f, 3.0f, 4.0f, 0.0f, "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(learningSpaceId));
        });
    }

    /// <summary>
    /// Verifies the service returns an empty list when the learning space has no components.
    /// </summary>
    [Test(Description = "Application-002: Returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var emptyComponents = new List<LearningComponent>();

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(emptyComponents);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies the service throws ArgumentException when the learning space ID is empty.
    /// </summary>
    [Test(Description = "Application-003: Throws ArgumentException when learning space ID is empty")]
    public void GetComponentsByLearningSpaceIdAsync_EmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        var invalidLearningSpaceId = string.Empty;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId));

        Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Verifies the service throws ArgumentException when the learning space ID is null.
    /// </summary>
    [Test(Description = "Application-004: Throws ArgumentException when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string nullLearningSpaceId = null!;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _sut.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId));

        Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
