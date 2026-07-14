using Microsoft.EntityFrameworkCore;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<DbSet<LearningComponent>> _mockDbSet;
    private Mock<UCRDatabaseContext> _mockDbContext;
    private SqlLearningComponentRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Returns list of components for a valid learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsList()
    {
        // Arrange
        string learningSpaceId = "SPACE-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.5f, 0.0f, 1.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1.0f, 1.0f, 1.0f, 2.0f, 0.0f, 1.0f, "South")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(components.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(components.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(components.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(components.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

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

    /// <summary>
    /// Verifies repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "SPACE-002";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

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

    /// <summary>
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Returns empty list when learning space ID does not exist")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "SPACE-999";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

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
