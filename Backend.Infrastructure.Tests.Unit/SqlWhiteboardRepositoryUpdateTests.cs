using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository"/> update-related methods.
/// Covers intents Infrastructure-001 through Infrastructure-004 for story CPD-LC-001-005.
/// </summary>
[TestFixture]
public class SqlWhiteboardRepositoryUpdateTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Whiteboard>> _mockDbSet = null!;
    private SqlWhiteboardRepository _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "LS-0103";

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
    /// a whiteboard by ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository retrieves whiteboard by ID successfully")]
    public async Task GetByIdAsync_ExistingId_ReturnsWhiteboard()
    {
        // Arrange
        var expectedWhiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        _mockDbSet
            .Setup(d => d.FindAsync(ValidComponentId))
            .ReturnsAsync(expectedWhiteboard);

        // Act
        var result = await _sut.GetByIdAsync(ValidComponentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ComponentId, Is.EqualTo(ValidComponentId));
            Assert.That(result.MarkerColor, Is.EqualTo("Blue"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository returns null
    /// when a whiteboard with the given ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns null when whiteboard not found")]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = "WB-NONEXISTENT";
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
    [Description("Infrastructure-003: Repository updates whiteboard and persists changes")]
    public async Task UpdateAsync_ValidWhiteboard_UpdatesAndCallsSaveChanges()
    {
        // Arrange
        var whiteboardToUpdate = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
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
                    w.ComponentId == ValidComponentId && w.MarkerColor == "Red")),
                Times.Once);
            _mockDbContext.Verify(
                c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-004: Verify that the repository retrieves all whiteboards/components
    /// in a given learning space.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Repository retrieves all components in a learning space")]
    public async Task GetByLearningSpaceIdAsync_ValidId_ReturnsAllComponents()
    {
        // Arrange
        var whiteboard1 = new Whiteboard(
            "WB-001", ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");
        var whiteboard2 = new Whiteboard(
            "WB-002", ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "South", "Green");
        var whiteboards = new List<Whiteboard> { whiteboard1, whiteboard2 }.AsQueryable();

        _mockDbContext.Setup(c => c.GetWhiteboardsQuery()).Returns(whiteboards);

        // Act
        var result = await _sut.GetByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        });
    }
}
