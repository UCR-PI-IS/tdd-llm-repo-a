namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private ApplicationDbContext _dbContext;
    private SqlLearningComponentRepository _repository;

    [SetUp]
    public void SetUp()
    {
        // Use in-memory database for testing
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(_dbContextOptions);
        _repository = new SqlLearningComponentRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync returns a list of components for a valid learning space ID.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithValidLearningSpaceId_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 3.0f, 1.5f, 1.0f, 2.0f, 0.5f, "North"),
            new LearningComponent("comp-002", learningSpaceId, 1.5f, 2.5f, 1.0f, 3.0f, 1.0f, 0.5f, "South")
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
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WhenSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "space-002";

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
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when the learning space ID does not exist.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "non-existent-space";

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
