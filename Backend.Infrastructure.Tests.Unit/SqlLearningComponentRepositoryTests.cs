using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository using EF Core InMemory database.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private UCRDatabaseContext _dbContext;
    private SqlLearningComponentRepository _repository;

    /// <summary>
    /// Sets up the test fixture with in-memory database.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _repository = new SqlLearningComponentRepository(_dbContext);
    }

    /// <summary>
    /// Cleans up the test fixture by disposing the database context.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Tests that repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = "SPACE-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 2.0f, 1.5f, 1.0f, 10.0f, 5.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 1.5f, 1.0f, 0.8f, 15.0f, 8.0f, 0.0f, "South")
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
    /// Tests that repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "SPACE-001";
        var otherSpaceId = "SPACE-002";
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", otherSpaceId, 2.0f, 1.5f, 1.0f, 10.0f, 5.0f, 0.0f, "North")
        };

        await _dbContext.LearningComponents.AddRangeAsync(components);
        await _dbContext.SaveChangesAsync();

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
    /// Tests that repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentSpace_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "NON-EXISTENT";

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
