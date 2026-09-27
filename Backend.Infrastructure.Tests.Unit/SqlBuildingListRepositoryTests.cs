using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingListRepository.GetAllBuildingsAsync"/>.
/// Covers intents Infrastructure-001 and Infrastructure-002.
/// </summary>
[TestFixture]
public class SqlBuildingListRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Building>> _mockDbSet = null!;
    private SqlBuildingListRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Building>>();

        _sut = new SqlBuildingListRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that the repository returns all buildings
    /// from the database context when buildings exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository returns all buildings from database when buildings exist")]
    public async Task GetAllBuildingsAsync_BuildingsExist_ReturnsAllBuildings()
    {
        // Arrange
        var expectedBuildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        }.AsQueryable();

        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Provider).Returns(expectedBuildings.Provider);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Expression).Returns(expectedBuildings.Expression);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.ElementType).Returns(expectedBuildings.ElementType);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.GetEnumerator()).Returns(expectedBuildings.GetEnumerator());

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(_mockDbSet.Object);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Engineering Building"));
            Assert.That(result[1].Name, Is.EqualTo("Science Building"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository returns an empty list
    /// when the database contains no buildings.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns empty list when database contains no buildings")]
    public async Task GetAllBuildingsAsync_NoBuildings_ReturnsEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>().AsQueryable();

        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Provider).Returns(emptyBuildings.Provider);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.Expression).Returns(emptyBuildings.Expression);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.ElementType).Returns(emptyBuildings.ElementType);
        _mockDbSet.As<IQueryable<Building>>()
            .Setup(m => m.GetEnumerator()).Returns(emptyBuildings.GetEnumerator());

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(_mockDbSet.Object);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }
}
