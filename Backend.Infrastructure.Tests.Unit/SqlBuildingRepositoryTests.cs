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
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Building>> _mockDbSet = null!;
    private SqlBuildingRepository _sut = null!;

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

    /// <summary>
    /// Infrastructure-001: Verify that a building is correctly persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Building is correctly persisted to the database")]
    public async Task AddAsync_ValidBuilding_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var building = new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        // Act
        var result = await _sut.AddAsync(building);

        // Assert
        Assert.That(result, Is.Not.Null);
        _mockDbSet.Verify(d => d.AddAsync(building, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
        var buildings = new List<Building>
        {
            new Building("Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1)
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.Provider).Returns(buildings.Provider);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.Expression).Returns(buildings.Expression);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.ElementType).Returns(buildings.ElementType);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.GetEnumerator()).Returns(() => buildings.GetEnumerator());

        _mockDbContext.Setup(c => c.Buildings).Returns(mockDbSet.Object);

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
        var buildings = new List<Building>().AsQueryable();

        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.Provider).Returns(buildings.Provider);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.Expression).Returns(buildings.Expression);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.ElementType).Returns(buildings.ElementType);
        mockDbSet.As<IQueryable<Building>>().Setup(m => m.GetEnumerator()).Returns(() => buildings.GetEnumerator());

        _mockDbContext.Setup(c => c.Buildings).Returns(mockDbSet.Object);

        // Act
        var result = await _sut.ExistsByNameAsync(buildingName);

        // Assert
        Assert.That(result, Is.False);
    }
}
