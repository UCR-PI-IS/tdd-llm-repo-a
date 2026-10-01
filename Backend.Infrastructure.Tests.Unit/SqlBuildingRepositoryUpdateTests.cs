using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/> update and read operations.
/// Covers intents Infrastructure-001 through Infrastructure-003 for PQL-AE-001-002.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryUpdateTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlBuildingRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlBuildingRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that repository retrieves an existing building from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository retrieves an existing building from the database")]
    public async Task GetByIdAsync_ExistingBuilding_ReturnsBuilding()
    {
        // Arrange
        var expectedBuilding = new Building(
            1, "Engineering Building", "Blue",
            10.5f, 20.0f, 15.0f,
            100.0f, 0.0f, 200.0f);

        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.Setup(d => d.FindAsync(1)).ReturnsAsync(expectedBuilding);

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

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
    /// Infrastructure-002: Verify that repository returns null when building ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns null when building ID does not exist")]
    public async Task GetByIdAsync_NonExistentBuilding_ReturnsNull()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.Setup(d => d.FindAsync(999)).ReturnsAsync((Building?)null);

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository successfully updates an existing building in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository successfully updates an existing building in the database")]
    public async Task UpdateAsync_ValidBuilding_UpdatesAndSaves()
    {
        // Arrange
        var buildingToUpdate = new Building(
            1, "Updated Building", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        var mockDbSet = new Mock<DbSet<Building>>();

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.UpdateAsync(buildingToUpdate);

        // Assert
        mockDbSet.Verify(
            d => d.Update(It.Is<Building>(b => b.InternalId == 1)),
            Times.Once);
        _mockDbContext.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
