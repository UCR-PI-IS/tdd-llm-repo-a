using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/> update-related operations.
/// Covers intents Infrastructure-001 through Infrastructure-003 for story PQL-AE-001-002.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryUpdateTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    // Valid test data
    private const int ValidInternalId = 1;
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
    /// Infrastructure-001: Verify that GetByIdAsync retrieves an existing building from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GetByIdAsync retrieves an existing building from the database")]
    public async Task GetByIdAsync_ExistingBuilding_ReturnsBuilding()
    {
        // Arrange
        var building = new Building(
            ValidInternalId, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(ValidInternalId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.InternalId, Is.EqualTo(ValidInternalId));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that GetByIdAsync returns null when the building ID
    /// does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: GetByIdAsync returns null when building ID does not exist")]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = 999;

        // Act
        var result = await _sut.GetByIdAsync(nonExistingId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that UpdateAsync successfully updates an existing building
    /// in the database. The changes should be persisted and retrievable.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: UpdateAsync successfully updates an existing building in the database")]
    public async Task UpdateAsync_ExistingBuilding_PersistsChanges()
    {
        // Arrange
        var building = new Building(
            ValidInternalId, "Old Name", "Red",
            10.0f, 20.0f, 15.0f,
            0f, 0f, 0f);
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Detach the entity to simulate a fresh update scenario
        _dbContext.Entry(building).State = EntityState.Detached;

        // Create a new building instance with updated values
        var buildingToUpdate = new Building(
            ValidInternalId, "Updated Name", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        // Act
        await _sut.UpdateAsync(buildingToUpdate);

        // Assert
        var updated = await _dbContext.Buildings.FindAsync(ValidInternalId);
        Assert.That(updated!.Name, Is.EqualTo("Updated Name"));
    }
}
