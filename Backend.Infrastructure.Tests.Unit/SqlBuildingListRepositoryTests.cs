using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingListRepository.GetAllBuildingsAsync"/>.
/// Covers intents Infrastructure-001 and Infrastructure-002.
/// Uses EF Core In-Memory database to avoid mocking unsupported DbSet interface implementations.
/// </summary>
[TestFixture]
public class SqlBuildingListRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingListRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _sut = new SqlBuildingListRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Infrastructure-001: Verify that the repository returns all buildings
    /// from the database context when buildings exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository returns all buildings when buildings exist")]
    public async Task GetAllBuildingsAsync_BuildingsExist_ReturnsAllBuildings()
    {
        // Arrange
        var buildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        };

        await _dbContext.Buildings.AddRangeAsync(buildings);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Engineering Building"));
            Assert.That(result[1].Name, Is.EqualTo("Science Building"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository returns an empty list
    /// when the database contains no buildings.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns empty list when no buildings exist")]
    public async Task GetAllBuildingsAsync_NoBuildingsExist_ReturnsEmptyList()
    {
        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }
}
