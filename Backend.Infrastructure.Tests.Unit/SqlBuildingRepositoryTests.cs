using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
/// Uses EF Core In-Memory database for testing.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlBuildingRepository _sut = null!;

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 15.0f;
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

    private Building CreateValidBuilding()
    {
        return new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
    }

    /// <summary>
    /// Infrastructure-001: Verify that a building is correctly persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Building is correctly persisted to the database")]
    public async Task AddAsync_ValidBuilding_PersistsToDatabase()
    {
        // Arrange
        var building = CreateValidBuilding();

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        Assert.That(result, Is.Not.Null);

        var added = await _dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == ValidName);
        Assert.That(added, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(added.Name, Is.EqualTo(ValidName));
            Assert.That(added.Color, Is.EqualTo(ValidColor));
            Assert.That(added.Height, Is.EqualTo(ValidHeight));
            Assert.That(added.Length, Is.EqualTo(ValidLength));
            Assert.That(added.Width, Is.EqualTo(ValidWidth));
            Assert.That(added.AreaId, Is.EqualTo(ValidAreaId));
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
        var building = CreateValidBuilding();
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync(ValidName);

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
        // Act
        var result = await _sut.ExistsByNameAsync("NonExistent Building");

        // Assert
        Assert.That(result, Is.False);
    }
}
