using Microsoft.EntityFrameworkCore;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> infrastructure repository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    // ---------- Positive tests ----------

    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database.")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, Orientation.North),
            new LearningComponent("comp-002", learningSpaceId, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 0.0f, Orientation.South)
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(components.Provider);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(components.Expression);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(components.ElementType);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(components.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    [Test]
    [Description("Verify repository returns empty list when learning space has no components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_SpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    // ---------- Negative tests ----------

    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database.")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }
}
