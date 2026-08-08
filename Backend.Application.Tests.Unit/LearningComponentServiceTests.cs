using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentService"/>.
/// Verifies the service correctly delegates to the repository and validates input.
/// </summary>
[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentListRepository> _mockRepository = null!;
    private LearningComponentService _service = null!;

    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentListRepository>();
        _service = new LearningComponentService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository = null!;
        _service = null!;
    }

    // ────────────────────────────────────────────────────────────────────
    // Application-001  –  Returns components when learning space has them
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Application-001: Verify service returns list of components when learning space has components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasComponents_ReturnsComponentList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", ValidLearningSpaceId, 2f, 1.5f, 0.5f, 1f, 2f, 0f, "North"),
            new LearningComponent("COMP-002", ValidLearningSpaceId, 3f, 2f, 1f, 3f, 4f, 0f, "South")
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
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
            Assert.That(result[1].LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Application-002  –  Returns empty list when no components exist
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Application-002: Verify service returns empty list when learning space has no components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await _service.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Application-003 & Application-004  –  Invalid learning space ID
    // ────────────────────────────────────────────────────────────────────

    [TestCase(null, Description = "Application-004: null learning space ID throws ArgumentException")]
    [TestCase("", Description = "Application-003: empty learning space ID throws ArgumentException")]
    [TestCase(" ", Description = "Application-003: whitespace learning space ID throws ArgumentException")]
    [Description("Application-003/004: Verify service throws ArgumentException when learning space ID is null, empty, or whitespace.")]
    public async Task GetComponentsByLearningSpaceIdAsync_InvalidLearningSpaceId_ThrowsArgumentException(string? invalidLearningSpaceId)
    {
        // Arrange & Act
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.GetComponentsByLearningSpaceIdAsync(invalidLearningSpaceId!));

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
