using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for LearningComponentRepository
/// User Story: CPD-LC-001-001 - List components in a learning space
/// </summary>
[TestFixture]
public class LearningComponentRepositoryTests
{
    private DbContextOptions<UCRDatabaseContext> _options;

    [SetUp]
    public void SetUp()
    {
        // Create a unique database name for each test to ensure isolation
        _options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Test]
    [Description("Repository should return all components for a valid learning space ID")]
    public async Task GetByLearningSpaceIdAsync_WithValidLearningSpaceId_ShouldReturnComponents()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var component1 = new LearningComponent(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North");
        var component2 = new LearningComponent(2, "LS-001", 3.0f, 2.0f, 0.3f, 2.0f, 1.0f, 4.0f, "South");
        context.LearningComponents.AddRange(component1, component2);
        await context.SaveChangesAsync();

        var repository = new LearningComponentRepository(context);

        // Act
        var result = await repository.GetByLearningSpaceIdAsync("LS-001");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    [Description("Repository should return empty list when learning space has no components")]
    public async Task GetByLearningSpaceIdAsync_WithNonExistentLearningSpaceId_ShouldReturnEmptyList()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var repository = new LearningComponentRepository(context);

        // Act
        var result = await repository.GetByLearningSpaceIdAsync("LS-999");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
    }

    [Test]
    [Description("Repository should filter components correctly by learning space ID")]
    public async Task GetByLearningSpaceIdAsync_WithMultipleLearningSpaces_ShouldReturnOnlyMatchingComponents()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var component1 = new LearningComponent(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North");
        var component2 = new LearningComponent(2, "LS-002", 3.0f, 2.0f, 0.3f, 2.0f, 1.0f, 4.0f, "South");
        var component3 = new LearningComponent(3, "LS-001", 1.5f, 1.0f, 0.2f, 0.5f, 0.0f, 1.5f, "East");
        context.LearningComponents.AddRange(component1, component2, component3);
        await context.SaveChangesAsync();

        var repository = new LearningComponentRepository(context);

        // Act
        var result = await repository.GetByLearningSpaceIdAsync("LS-001");
        var resultList = result.ToList();

        // Assert
        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList.All(c => c.LearningSpaceId == "LS-001"), Is.True);
    }
}
