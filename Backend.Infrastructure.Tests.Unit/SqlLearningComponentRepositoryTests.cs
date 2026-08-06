using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<AppDbContext> _mockContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<AppDbContext>();
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _repository = new SqlLearningComponentRepository(_mockContext.Object);
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithExistingComponents_ReturnsComponentList()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001", learningSpaceId, 2.0f, 3.0f, 1.0f, 10.0f, 5.0f, 0.0f, "North"),
            new LearningComponent(
                "comp-002", learningSpaceId, 1.5f, 2.5f, 1.0f, 15.0f, 8.0f, 0.0f, "South")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(components.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(components.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(components.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(components.GetEnumerator());

        _mockContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-empty";
        var emptyComponents = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(emptyComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(emptyComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(emptyComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(emptyComponents.GetEnumerator());

        _mockContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentLearningSpace_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";
        var existingComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001", "space-other", 2.0f, 3.0f, 1.0f, 10.0f, 5.0f, 0.0f, "North")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(existingComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(existingComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(existingComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(existingComponents.GetEnumerator());

        _mockContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
    }
}
