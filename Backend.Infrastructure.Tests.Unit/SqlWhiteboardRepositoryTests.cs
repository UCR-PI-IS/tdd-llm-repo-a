using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
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
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.5f;
    private const float ValidZ = 1.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

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

    private static Whiteboard CreateValidWhiteboard()
    {
        return new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    /// <summary>
    /// Infrastructure-001: Verify that the repository adds a whiteboard to the DbSet
    /// using AddAsync and calls SaveChangesAsync to persist it to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds whiteboard to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidWhiteboard_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var whiteboard = CreateValidWhiteboard();

        // Act
        await _sut.AddAsync(whiteboard);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(
                It.Is<Whiteboard>(w =>
                    w.ComponentId == ValidComponentId &&
                    w.LearningSpaceId == ValidLearningSpaceId &&
                    w.MarkerColor == ValidMarkerColor),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository wraps DbUpdateException in a
    /// DatabaseException when SaveChangesAsync fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository throws DatabaseException when SaveChanges fails")]
    public async Task AddAsync_SaveChangesFails_ThrowsDatabaseException()
    {
        // Arrange
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));

        var whiteboard = CreateValidWhiteboard();

        // Act
        DatabaseException? caughtException = null;
        try
        {
            await _sut.AddAsync(whiteboard);
        }
        catch (DatabaseException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DatabaseException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Database error"));
        });
    }
}
