using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository.UpdateAsync"/>.
/// Covers intent Infrastructure-003.
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
    /// Infrastructure-003: Verify that repository successfully updates a whiteboard in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository successfully updates whiteboard in database")]
    public async Task UpdateAsync_ValidWhiteboard_UpdatesInDatabase()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "IF-0103";
        var whiteboardToUpdate = new Whiteboard(
            whiteboardId,
            learningSpaceId,
            3.0f,
            2.0f,
            0.2f,
            2.0f,
            1.0f,
            3.0f,
            "East",
            "Red");

        // Act
        await _sut.UpdateAsync(whiteboardToUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.Update(It.Is<Whiteboard>(w => w.ComponentId == whiteboardId && w.MarkerColor == "Red")), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }
}
