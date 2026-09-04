using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository.AddAsync"/>.
/// Covers intents Infrastructure-001 through Infrastructure-002.
/// </summary>
[TestFixture]
public class SqlWhiteboardRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Whiteboard>> _mockDbSet = null!;
    private SqlWhiteboardRepository _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Whiteboard>>();
        _mockDbContext.Setup(c => c.Whiteboards).Returns(_mockDbSet.Object);
        _sut = new SqlWhiteboardRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify repository successfully adds whiteboard to database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Successfully add whiteboard to database")]
    public async Task AddAsync_ValidWhiteboard_AddsToDatabase()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId,
            ValidLearningSpaceId,
            2.0f, 1.0f, 0.5f,
            0.0f, 0.0f, 0.0f,
            "North",
            "Blue");

        _mockDbSet
            .Setup(d => d.AddAsync(whiteboard, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EntityEntry<Whiteboard>>((EntityEntry<Whiteboard>)null!));

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.AddAsync(whiteboard);

        // Assert
        _mockDbSet.Verify(d => d.AddAsync(whiteboard, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-002: Verify repository throws DatabaseException when database save fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Throw exception when database save fails")]
    public void AddAsync_DatabaseSaveFails_ThrowsDatabaseException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId,
            ValidLearningSpaceId,
            2.0f, 1.0f, 0.5f,
            0.0f, 0.0f, 0.0f,
            "North",
            "Blue");

        _mockDbSet
            .Setup(d => d.AddAsync(whiteboard, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EntityEntry<Whiteboard>>((EntityEntry<Whiteboard>)null!));

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DatabaseException>(async () => await _sut.AddAsync(whiteboard));
        Assert.That(ex.Message, Does.Contain("Database error"));
    }
}
