using Moq;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// Tests the GetComponentsByLearningSpaceIdAsync method with a mocked DbContext.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>().Options;
        _mockContext = new Mock<UCRDatabaseContext>(options);
        _repository = new SqlLearningComponentRepository(_mockContext.Object);
    }

    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database (Infrastructure-001)")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidId_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2f, 3f, 1f, 5f, 10f, 0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1f, 2f, 0.5f, 8f, 12f, 0f, "South")
        };
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(components);
        _mockContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    [Test]
    [Description("Verify repository returns empty list when learning space has no components (Infrastructure-002)")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS-002";
        var components = new List<LearningComponent>();
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(components);
        _mockContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database (Infrastructure-003)")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "NON-EXISTENT";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", "LS-001", 2f, 3f, 1f, 5f, 10f, 0f, "North")
        };
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(components);
        _mockContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }
}
