using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Building>> _mockDbSet = null!;
    private SqlBuildingRepository _sut = null!;

    // Valid test data
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Building>>();

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(_mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new SqlBuildingRepository(_mockDbContext.Object);
    }

    private Building CreateValidBuilding()
    {
        return new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);
    }

    /// <summary>
    /// Infrastructure-001: Verify that a building is correctly persisted to the database.
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
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            _mockDbSet.Verify(d => d.Add(building), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking for existing building by name returns true when building exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns true when building with given name exists")]
    public async Task ExistsByNameAsync_BuildingExists_ReturnsTrue()
    {
        // Arrange
        var buildingName = ValidName;
        var buildings = new List<Building>
        {
            CreateValidBuilding()
        }.AsQueryable();

        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Provider)
            .Returns(buildings.Provider);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Expression)
            .Returns(buildings.Expression);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.ElementType)
            .Returns(buildings.ElementType);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.GetEnumerator())
            .Returns(buildings.GetEnumerator());

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking for existing building by name returns false when building does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository returns false when building with given name does not exist")]
    public async Task ExistsByNameAsync_BuildingDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var buildingName = "NonExistent Building";
        var buildings = new List<Building>
        {
            CreateValidBuilding()
        }.AsQueryable();

        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Provider)
            .Returns(buildings.Provider);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Expression)
            .Returns(buildings.Expression);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.ElementType)
            .Returns(buildings.ElementType);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.GetEnumerator())
            .Returns(buildings.GetEnumerator());

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.False);
    }
}
