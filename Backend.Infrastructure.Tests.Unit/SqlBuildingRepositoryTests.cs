using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003 for the Add Building story.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;
    private const int ValidAreaId = 1;

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
    /// via AddAsync. The method should return the persisted building entity.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: AddAsync persists building and returns the entity")]
    public async Task AddAsync_ValidBuilding_PersistsAndReturnsBuilding()
    {
        // Arrange
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(ValidName));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true when a building
    /// with the specified name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when building exists")]
    public async Task ExistsByNameAsync_BuildingExists_ReturnsTrue()
    {
        // Arrange
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync(ValidName);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false when no building
    /// with the specified name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when building does not exist")]
    public async Task ExistsByNameAsync_BuildingDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentName = "Non-Existent Building";

        // Act
        var result = await _sut.ExistsByNameAsync(nonExistentName);

        // Assert
        Assert.That(result, Is.False);
    }
}
