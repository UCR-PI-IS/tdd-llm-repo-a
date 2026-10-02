using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/> update-related operations.
/// Covers intents Infrastructure-001 through Infrastructure-003 for the Edit Building story (PQL-AE-001-002).
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryUpdateTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;

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
    /// Infrastructure-001: Verify that GetByIdAsync retrieves an existing building from the database
    /// with the correct internal identifier.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GetByIdAsync retrieves an existing building from the database")]
    public async Task GetByIdAsync_ExistingBuilding_ReturnsBuilding()
    {
        // Arrange
        var building = new Building(
            1, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.InternalId, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that GetByIdAsync returns null when the building ID
    /// does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: GetByIdAsync returns null when building ID does not exist")]
    public async Task GetByIdAsync_NonExistingBuilding_ReturnsNull()
    {
        // Arrange
        var nonExistingId = 999;

        // Act
        var result = await _sut.GetByIdAsync(nonExistingId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that UpdateAsync successfully persists the updated building
    /// data to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: UpdateAsync persists updated building to the database")]
    public async Task UpdateAsync_ValidBuilding_PersistsChanges()
    {
        // Arrange
        var building = new Building(
            1, "Old Name", "Red",
            10.0f, 20.0f, 15.0f,
            0f, 0f, 0f);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        var updatedBuilding = new Building(
            1, "Updated Name", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        // Act
        await _sut.UpdateAsync(updatedBuilding);

        // Assert
        var persisted = await _dbContext.Buildings.FindAsync(1);
        Assert.That(persisted!.Name, Is.EqualTo("Updated Name"));
    }
}
