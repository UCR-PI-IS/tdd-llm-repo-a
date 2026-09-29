using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
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

    /// <summary>
    /// Infrastructure-001: Verify that a building is correctly persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository persists building to database correctly")]
    public async Task AddAsync_ValidBuilding_PersistsToDatabase()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        Assert.That(result, Is.Not.Null);
        var persisted = await _dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == "Engineering Building");
        Assert.That(persisted, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(persisted.Name, Is.EqualTo("Engineering Building"));
            Assert.That(persisted.Color, Is.EqualTo("Red"));
            Assert.That(persisted.Height, Is.EqualTo(20.5f));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking for existing building by name returns true when building exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when building exists")]
    public async Task ExistsByNameAsync_BuildingExists_ReturnsTrue()
    {
        // Arrange
        var buildingName = "Engineering Building";
        var building = new Building(buildingName, "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking for existing building by name returns false when building does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when building does not exist")]
    public async Task ExistsByNameAsync_BuildingDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var buildingName = "NonExistent Building";

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.False);
    }
}
