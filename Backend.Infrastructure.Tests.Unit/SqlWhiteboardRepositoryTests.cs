using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
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
    /// Infrastructure-001: Verify that the repository adds a whiteboard to the DbSet
    /// via AddAsync and calls SaveChangesAsync to persist it to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds whiteboard to database and calls SaveChanges")]
    public async Task AddAsync_ValidWhiteboard_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            "WB-001", "IF-0103",
            2.0f, 1.0f, 0.1f,
            1.0f, 0.5f, 0.0f,
            "South", "Blue");

        // Act
        await _sut.AddAsync(whiteboard);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(
                It.Is<Whiteboard>(w => w.ComponentId == "WB-001" && w.MarkerColor == "Blue"),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository wraps DbUpdateException
    /// in a DatabaseException when SaveChangesAsync fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository throws DatabaseException when database save fails")]
    public async Task AddAsync_SaveChangesFails_ThrowsDatabaseException()
    {
        // Arrange
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));

        var whiteboard = new Whiteboard(
            "WB-001", "IF-0103",
            2.0f, 1.0f, 0.1f,
            1.0f, 0.5f, 0.0f,
            "South", "Blue");

        // Act & Assert
        var ex = Assert.ThrowsAsync<DatabaseException>(() => _sut.AddAsync(whiteboard));
        Assert.That(ex!.Message, Does.Contain("Database error"));
    }
}
