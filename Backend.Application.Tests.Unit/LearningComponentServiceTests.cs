namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Unit tests for the LearningComponentService class.
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

    [TearDown]
    public void TearDown()
    {
        _mockRepository.Verify();
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync returns a list of components when the learning space has components.
    /// </summary>
    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WhenSpaceHasComponents_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 3.0f, 1.5f, 1.0f, 2.0f, 0.5f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.5f, 1.0f, 3.0f, 1.0f, 0.5f, "South")
        };

        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WhenSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";
        var emptyList = new List<LearningComponent>();

        _mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
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
    /// Tests that GetComponentsByLearningSpaceIdAsync throws ArgumentException when the learning space ID is empty.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_WithEmptyLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string invalidLearningSpaceId = "";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId).GetAwaiter().GetResult());
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync throws ArgumentException when the learning space ID is null.
    /// </summary>
    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_WithNullLearningSpaceId_ThrowsArgumentException()
    {
        // Arrange
        string? nullLearningSpaceId = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            _service.GetComponentsByLearningSpaceIdAsync(nullLearningSpaceId!).GetAwaiter().GetResult());
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
