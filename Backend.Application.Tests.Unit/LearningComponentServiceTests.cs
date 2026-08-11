using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponentService"/> class.
/// Tests the service layer logic for retrieving learning components by learning space ID.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentListRepository> _mockRepository = null!;
    private LearningComponentService _service = null!;

    private const string ValidLearningSpaceId = "ls-001";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentListRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify service returns a list of components when the learning space
    /// has components. The service should delegate to the repository and return the result.
    /// </summary>
    [Test]
    [Description("Application-001: Returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsComponentList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", ValidLearningSpaceId, 2f, 3f, 1.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("comp-002", ValidLearningSpaceId, 1f, 2f, 1f, 5f, 10f, 0f, "South")
        };

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
        });
    }

    /// <summary>
    /// Application-002: Verify service returns an empty list when the learning space
    /// has no components. The repository returns an empty list and the service passes it through.
    /// </summary>
    [Test]
    [Description("Application-002: Returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var emptyLearningSpaceId = "ls-002";

        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(emptyLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(emptyLearningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Application-003 and Application-004: Verify service throws ArgumentException when
    /// the learning space ID is null or empty. Tests both null and empty string inputs.
    /// </summary>
    [TestCase("", Description = "Application-003: Empty string learning space ID throws ArgumentException")]
    [TestCase(null, Description = "Application-004: Null learning space ID throws ArgumentException")]
    public void GetComponentsByLearningSpaceIdAsync_InvalidLearningSpaceId_ThrowsArgumentException(
        string? invalidLearningSpaceId)
    {
        // Act & Assert
        Assert.That(
            async () => await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId!),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("learningSpaceId"));
    }
}
