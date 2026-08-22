using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningSpaceRepository.AddAsync"/>.
/// Covers intents Infrastructure-001 through Infrastructure-004 for story SQL-LS-001-007.
/// </summary>
[TestFixture]
public class SqlLearningSpaceRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningSpace>> _mockDbSet = null!;
    private SqlLearningSpaceRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningSpace>>();
        _mockDbContext.Setup(c => c.LearningSpaces).Returns(_mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _sut = new SqlLearningSpaceRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001 and Infrastructure-002: Verify that the repository adds a learning space
    /// to the DbSet and calls SaveChangesAsync to persist it to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001/002: AddAsync adds entity to DbSet and calls SaveChangesAsync")]
    public async Task AddAsync_ValidLearningSpace_AddsToDbSetAndSavesChanges()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await _sut.AddAsync(learningSpace);

        // Assert
        _mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls =>
            ls.Type == "Classroom" && ls.Height == 3.0f && ls.Width == 8.0f && ls.Length == 10.0f)),
            Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-003: Verify that the repository propagates DbUpdateException
    /// when SaveChangesAsync fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: AddAsync propagates DbUpdateException when SaveChangesAsync fails")]
    public async Task AddAsync_SaveChangesFails_PropagatesDbUpdateException()
    {
        // Arrange
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
            await _sut.AddAsync(learningSpace));
        Assert.That(ex!.Message, Does.Contain("Database error"));
    }

    /// <summary>
    /// Infrastructure-004: Verify that multiple learning spaces can be added sequentially,
    /// with each add operation calling DbSet.Add and SaveChangesAsync.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Multiple learning spaces can be added sequentially")]
    public async Task AddAsync_MultipleLearningSpaces_AddsAllToDbSetAndSavesChanges()
    {
        // Arrange
        var space1 = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);
        var space2 = new LearningSpace("Laboratory", 3.5f, 12.0f, 15.0f);

        // Act
        await _sut.AddAsync(space1);
        await _sut.AddAsync(space2);

        // Assert
        _mockDbSet.Verify(d => d.Add(It.IsAny<LearningSpace>()), Times.Exactly(2));
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
