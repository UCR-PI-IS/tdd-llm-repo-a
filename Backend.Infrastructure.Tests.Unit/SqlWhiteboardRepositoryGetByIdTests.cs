using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository.GetByIdAsync"/>.
/// Covers intents Infrastructure-001 and Infrastructure-002.
/// </summary>
[TestFixture]
public class SqlWhiteboardRepositoryGetByIdTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Whiteboard>> _mockDbSet = null!;
    private SqlWhiteboardRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Whiteboard>>();

        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(_mockDbSet.Object);

        _sut = new SqlWhiteboardRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that repository successfully retrieves a whiteboard by ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository successfully retrieves whiteboard by ID")]
    public async Task GetByIdAsync_ExistingWhiteboard_ReturnsWhiteboard()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "IF-0103";
        var expectedWhiteboard = new Whiteboard(
            whiteboardId,
            learningSpaceId,
            2.0f,
            1.5f,
            0.1f,
            1.0f,
            0.0f,
            2.0f,
            "South",
            "Blue");

        _mockDbSet
            .Setup(d => d.FindAsync(whiteboardId))
            .ReturnsAsync(expectedWhiteboard);

        // Act
        var result = await _sut.GetByIdAsync(whiteboardId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ComponentId, Is.EqualTo(whiteboardId));
            Assert.That(result.MarkerColor, Is.EqualTo("Blue"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that repository returns null when whiteboard with given ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns null when whiteboard not found")]
    public async Task GetByIdAsync_NonExistentWhiteboard_ReturnsNull()
    {
        // Arrange
        var nonExistentId = "WB-NOTFOUND";

        _mockDbSet
            .Setup(d => d.FindAsync(nonExistentId))
            .ReturnsAsync((Whiteboard?)null);

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }
}
