using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003 for story PQL-AE-001-001.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    private const string BuildingName = "Engineering Building";
    private const string BuildingColor = "Red";
    private const float BuildingHeight = 20.5f;
    private const float BuildingLength = 50.0f;
    private const float BuildingWidth = 30.0f;
    private const float BuildingX = 100.0f;
    private const float BuildingY = 200.0f;
    private const float BuildingZ = 0.0f;
    private const int BuildingAreaId = 1;

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
    /// via AddAsync and can be retrieved afterward.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: AddAsync persists building to the database")]
    public async Task AddAsync_ValidBuilding_PersistsBuildingToDatabase()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        var persistedBuildings = await _dbContext.Buildings.ToListAsync();
        Assert.That(persistedBuildings, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true when a building
    /// with the given name already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when building exists")]
    public async Task ExistsByNameAsync_BuildingWithNameExists_ReturnsTrue()
    {
        // Arrange
        var building = new Building(BuildingName, BuildingColor, BuildingHeight, BuildingLength,
            BuildingWidth, BuildingX, BuildingY, BuildingZ, BuildingAreaId);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync(BuildingName);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false when no building
    /// with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when building does not exist")]
    public async Task ExistsByNameAsync_BuildingWithNameDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentName = "Non Existent Building";

        // Act
        var result = await _sut.ExistsByNameAsync(nonExistentName);

        // Assert
        Assert.That(result, Is.False);
    }
}
