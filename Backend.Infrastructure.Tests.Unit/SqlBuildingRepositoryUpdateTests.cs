using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingRepository"/> update operations.
/// Covers intents Infrastructure-001 through Infrastructure-003 for the Edit Building story (PQL-AE-001-002).
/// Uses EF Core In-Memory database and mocking for isolated unit testing.
/// </summary>
[TestFixture]
public class SqlBuildingRepositoryUpdateTests
{
    private Mock<UCRDatabaseContext> _mockContext = null!;
    private Mock<DbSet<Building>> _mockDbSet = null!;
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
        _mockContext = new Mock<UCRDatabaseContext>(new DbContextOptions<UCRDatabaseContext>());
        _mockDbSet = new Mock<DbSet<Building>>();
        _mockContext.Setup(c => c.Buildings).Returns(_mockDbSet.Object);
        _sut = new SqlBuildingRepository(_mockContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockContext.VerifyAll();
    }

    /// <summary>
    /// Infrastructure-001: Verify that repository retrieves an existing building from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GetByIdAsync returns existing building from database")]
    public async Task GetByIdAsync_ExistingBuilding_ReturnsBuilding()
    {
        // Arrange
        var expectedBuilding = new Building(
            ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);

        _mockDbSet.Setup(d => d.FindAsync(ValidInternalId)).ReturnsAsync(expectedBuilding);

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
    /// Infrastructure-002: Verify that repository returns null when building ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: GetByIdAsync returns null for non-existing building ID")]
    public async Task GetByIdAsync_NonExistingBuilding_ReturnsNull()
    {
        // Arrange
        var nonExistentId = 999;
        _mockDbSet.Setup(d => d.FindAsync(nonExistentId)).ReturnsAsync((Building?)null);

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository successfully updates an existing building in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: UpdateAsync persists updated building to database")]
    public async Task UpdateAsync_ExistingBuilding_PersistsChanges()
    {
        // Arrange
        var buildingToUpdate = new Building(
            ValidInternalId, "Updated Name", "Green", 15.0f, 25.0f, 20.0f, 150.0f, 10.0f, 250.0f);

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _sut.UpdateAsync(buildingToUpdate);

        // Assert
        _mockDbSet.Verify(d => d.Update(It.Is<Building>(b => b.InternalId == ValidInternalId)), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
