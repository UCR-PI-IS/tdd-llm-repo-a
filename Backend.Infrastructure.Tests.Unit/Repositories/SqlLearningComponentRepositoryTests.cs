using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    /// <summary>
    /// Sets up the test context with an in-memory database and repository instance.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _repository = new SqlLearningComponentRepository(_dbContext);
    }

    /// <summary>
    /// Cleans up the test context by disposing the database context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Verifies that the repository returns a list of components for a valid learning space ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify repository returns list of components for a valid learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithValidId_ReturnsComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.0f, 1.0f, 2.0f, 0.0f, 0.0f, "South")
        };

        await _dbContext.LearningComponents.AddRangeAsync(components);
        await _dbContext.SaveChangesAsync();

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
    /// Verifies that the repository returns an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";
        // No components added for this learning space

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
    /// Verifies that the repository returns an empty list when the learning space ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify repository returns empty list when learning space ID does not exist")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "space-nonexistent";
        // Add some components for other learning spaces
        var otherComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", "space-001", 2.0f, 1.5f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("comp-002", "space-002", 1.5f, 2.0f, 1.0f, 2.0f, 0.0f, 0.0f, "South")
        };
        await _dbContext.LearningComponents.AddRangeAsync(otherComponents);
        await _dbContext.SaveChangesAsync();

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
