using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private ILearningComponentRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>(MockBehavior.Strict, new DbContextOptions<UCRDatabaseContext>());
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithValidId_ReturnsComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 3.0f, 2.0f, 1.0f, 0.5f, 1.0f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.5f, 1.5f, 2.0f, 0.5f, 2.0f, "South")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(components.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(components.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(components.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(components.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);

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
        string learningSpaceId = "space-002";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);

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
        string nonExistentLearningSpaceId = "non-existent-space";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);

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
