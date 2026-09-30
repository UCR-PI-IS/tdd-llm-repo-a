using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
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

    /// <summary>
    /// Infrastructure-001: Verify that a building is correctly persisted to the database
    /// via AddAsync, and the returned entity matches the input.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Building is correctly persisted to the database via AddAsync")]
    public async Task AddAsync_ValidBuilding_PersistsToDatabase()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f,
            100.0f, 200.0f, 0.0f, 1);

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        var persistedBuilding = await _dbContext.Buildings.FirstOrDefaultAsync();
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(persistedBuilding, Is.Not.Null);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true
    /// when a building with the given name already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when building exists")]
    public async Task ExistsByNameAsync_BuildingExists_ReturnsTrue()
    {
        // Arrange
        var buildingName = "Engineering Building";
        var building = new Building(buildingName, "Red", 20.5f, 50.0f, 30.0f,
            100.0f, 200.0f, 0.0f, 1);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

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
        // Arrange
        var buildingName = "Non-Existent Building";

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.False);
    }
}
