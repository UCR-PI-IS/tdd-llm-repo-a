using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
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
    private SqlWhiteboardRepository _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlWhiteboardRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify repository adds whiteboard to database and persists changes.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Successfully add whiteboard to database")]
    public async Task AddAsync_ValidWhiteboard_AddsAndSaves()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.5f, 1.5f, 0.5f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);

        // Act
        await _sut.AddAsync(whiteboard);

        // Assert
        mockDbSet.Verify(
            d => d.AddAsync(whiteboard, It.IsAny<CancellationToken>()),
            Times.Once);
        _mockDbContext.Verify(
            c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Infrastructure-002: Verify repository throws DatabaseException when database save fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Throw exception when database save fails")]
    public async Task AddAsync_DatabaseSaveFails_ThrowsDatabaseException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.5f, 1.5f, 0.5f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

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
