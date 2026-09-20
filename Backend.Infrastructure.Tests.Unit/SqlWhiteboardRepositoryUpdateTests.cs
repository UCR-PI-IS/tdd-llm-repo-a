using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository"/> read and update operations.
/// Covers intents Infrastructure-001 through Infrastructure-004.
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

    /// <summary>
    /// Infrastructure-001: Verify that the repository successfully retrieves
    /// a whiteboard by its ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: GetByIdAsync retrieves whiteboard by ID from database")]
    public async Task GetByIdAsync_ExistingWhiteboard_ReturnsWhiteboard()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "IF-0103";
        var expectedWhiteboard = new Whiteboard(
            whiteboardId, learningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "South", "Blue");

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
    /// Infrastructure-002: Verify that the repository returns null
    /// when a whiteboard with the given ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: GetByIdAsync returns null when whiteboard does not exist")]
    public async Task GetByIdAsync_NonExistentWhiteboard_ReturnsNull()
    {
        // Arrange
        var nonExistentId = "WB-999";
        _mockDbSet
            .Setup(d => d.FindAsync(nonExistentId))
            .ReturnsAsync((Whiteboard?)null);

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that the repository successfully updates
    /// a whiteboard in the database and calls SaveChangesAsync.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: UpdateAsync updates whiteboard and persists changes")]
    public async Task UpdateAsync_ValidWhiteboard_UpdatesAndSavesChanges()
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
            _mockDbSet.Verify(
                d => d.Update(It.Is<Whiteboard>(w =>
                    w.ComponentId == whiteboardId &&
                    w.MarkerColor == "Red")),
                Times.Once);
            _mockDbContext.Verify(
                c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-004: Verify that the repository retrieves all whiteboards
    /// and components that belong to a specific learning space.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: GetByLearningSpaceIdAsync retrieves all components in a learning space")]
    public async Task GetByLearningSpaceIdAsync_ExistingComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = "IF-0103";
        var whiteboard1 = new Whiteboard(
            "WB-001", learningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "South", "Blue");
        var whiteboard2 = new Whiteboard(
            "WB-002", learningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "South", "Green");

        var whiteboards = new List<Whiteboard> { whiteboard1, whiteboard2 }.AsQueryable();

        _mockDbContext
            .Setup(c => c.GetWhiteboardsQuery())
            .Returns(whiteboards);

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
