using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService.GetComponentsByLearningSpaceIdAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;
    private LearningComponentService _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
        _sut = new LearningComponentService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service returns list of components when learning space has components.
    /// </summary>
    [Test]
    [Description("Application-001: Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 3.0f, 2.0f, 1.0f, 15f, 25f, 0f, "South")
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
    /// Application-002: Verify service returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Application-002: Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithNoComponents_ReturnsEmptyList()
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
            Assert.That(result, Has.Count.EqualTo(0));
        });
    }

    /// <summary>
    /// Application-003 and Application-004: Verify service throws ArgumentException when
    /// learning space ID is null or empty.
    /// </summary>
    [TestCase("", Description = "Application-003: Empty string learning space ID throws ArgumentException")]
    [TestCase(null, Description = "Application-004: Null learning space ID throws ArgumentException")]
    public async Task GetComponentsByLearningSpaceIdAsync_InvalidId_ThrowsArgumentException(string? invalidLearningSpaceId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId!);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("learningSpaceId"));
        });
    }
}
