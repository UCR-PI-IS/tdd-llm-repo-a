using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/> update and read operations.
/// Covers intents Infrastructure-001 through Infrastructure-003 for story PQL-AE-001-002.
/// Uses Moq for EF Core DbContext and DbSet to verify interaction patterns.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryUpdateTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
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
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlBuildingRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that repository retrieves an existing building
    /// from the database by its internal identifier.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GetByIdAsync returns existing building from database")]
    public async Task GetByIdAsync_ExistingBuilding_ReturnsBuilding()
    {
        // Arrange
        var expectedBuilding = new Building(
            ValidInternalId, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.Setup(d => d.FindAsync(ValidInternalId)).ReturnsAsync(expectedBuilding);

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByIdAsync(ValidInternalId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.InternalId, Is.EqualTo(ValidInternalId));
            Assert.That(result.Name, Is.EqualTo(ValidName));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that repository returns null when the requested
    /// building identifier does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: GetByIdAsync returns null for non-existing building id")]
    public async Task GetByIdAsync_NonExistingBuilding_ReturnsNull()
    {
        // Arrange
        var nonExistentId = 999;
        var mockDbSet = new Mock<DbSet<Building>>();
        mockDbSet.Setup(d => d.FindAsync(nonExistentId)).ReturnsAsync((Building?)null);

        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository successfully updates an existing building
    /// in the database by calling DbSet.Update and persisting changes via SaveChangesAsync.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: UpdateAsync calls DbSet.Update and SaveChangesAsync")]
    public async Task UpdateAsync_ValidBuilding_UpdatesAndSaves()
    {
        // Arrange
        var buildingToUpdate = new Building(
            ValidInternalId, "Updated Name", "Red",
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
        Assert.Multiple(() =>
        {
            mockDbSet.Verify(
                d => d.Update(It.Is<Building>(b => b.InternalId == ValidInternalId)),
                Times.Once);
            _mockDbContext.Verify(
                c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }
}
