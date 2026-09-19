using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository"/> update and query operations.
/// Covers intents Infrastructure-001 through Infrastructure-004 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class SqlWhiteboardRepositoryUpdateTests
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

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new SqlWhiteboardRepository(_mockDbContext.Object);
    }

    private static Whiteboard CreateWhiteboard(
        string componentId = "WB-001",
        string learningSpaceId = "IF-0103",
        string markerColor = "Blue")
    {
        return new Whiteboard(
            componentId, learningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", markerColor);
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
        var expectedWhiteboard = CreateWhiteboard(whiteboardId, learningSpaceId, "Blue");

        _mockDbSet
            .Setup(d => d.FindAsync(whiteboardId))
            .ReturnsAsync(expectedWhiteboard);

        // Act
        var result = await _sut.GetByIdAsync(whiteboardId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ComponentId, Is.EqualTo(whiteboardId));
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

    /// <summary>
    /// Infrastructure-003: Verify that repository successfully updates a whiteboard in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository successfully updates whiteboard")]
    public async Task UpdateAsync_ValidWhiteboard_UpdatesAndSaves()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "IF-0103";
        var whiteboardToUpdate = new Whiteboard(
            whiteboardId, learningSpaceId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        // Act
        await _sut.UpdateAsync(whiteboardToUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.Update(
                It.Is<Whiteboard>(w =>
                    w.ComponentId == whiteboardId &&
                    w.MarkerColor == "Red" &&
                    w.Width == 3.0f)),
                Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-004: Verify that repository retrieves all whiteboards/components in a learning space.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Repository retrieves all components in a learning space")]
    public async Task GetByLearningSpaceIdAsync_ExistingComponents_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = "IF-0103";
        var whiteboard1 = CreateWhiteboard("WB-001", learningSpaceId, "Blue");
        var whiteboard2 = CreateWhiteboard("WB-002", learningSpaceId, "Green");
        var whiteboards = new List<Whiteboard> { whiteboard1, whiteboard2 }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        mockDbSet.As<IQueryable<Whiteboard>>().Setup(m => m.Provider).Returns(whiteboards.Provider);
        mockDbSet.As<IQueryable<Whiteboard>>().Setup(m => m.Expression).Returns(whiteboards.Expression);
        mockDbSet.As<IQueryable<Whiteboard>>().Setup(m => m.ElementType).Returns(whiteboards.ElementType);
        mockDbSet.As<IQueryable<Whiteboard>>().Setup(m => m.GetEnumerator()).Returns(whiteboards.GetEnumerator());

        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        });
    }
}
