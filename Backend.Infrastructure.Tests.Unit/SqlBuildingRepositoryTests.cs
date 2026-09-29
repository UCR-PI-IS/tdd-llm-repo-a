using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003 for PQL-AE-001-001.
/// Uses EF Core In-Memory database to avoid mocking unsupported DbSet interface implementations.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _sut = new SqlBuildingRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    private static Building CreateValidBuilding() =>
        new("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

    /// <summary>
    /// Infrastructure-001: Verify that AddAsync adds a building to the DbSet,
    /// calls SaveChangesAsync, and returns the persisted building.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds building to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidBuilding_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var building = CreateValidBuilding();

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        Assert.That(result, Is.Not.Null);
        var added = await _dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == "Engineering Building");
        Assert.That(added, Is.Not.Null);
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true
    /// when a building with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when building exists")]
    public async Task ExistsByNameAsync_BuildingExists_ReturnsTrue()
    {
        // Arrange
        var building = CreateValidBuilding();
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync("Engineering Building");

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false
    /// when no building with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when building does not exist")]
    public async Task ExistsByNameAsync_BuildingDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.ExistsByNameAsync("Engineering Building");

        // Assert
        Assert.That(result, Is.False);
    }
}
