using Microsoft.EntityFrameworkCore;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    /// <summary>
    /// Sets up the test context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsList()
    {
        // Arrange
        String learningSpaceId = "LS-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 3.0f, 2.0f, 1.5f, 5.0f, 0.0f, 2.0f, "East")
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
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        String learningSpaceId = "LS-002";
        var components = new List<LearningComponent>().AsQueryable();

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
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentId_ReturnsEmptyList()
    {
        // Arrange
        String nonExistentLearningSpaceId = "LS-NOTEXIST";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", "LS-001", 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(components.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(components.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(components.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(components.GetEnumerator());
        
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
