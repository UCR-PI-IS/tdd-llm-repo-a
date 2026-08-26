using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningSpaceRepository.AddAsync"/>.
/// Covers intents Infrastructure-001 through Infrastructure-004.
/// </summary>
[TestFixture]
public class SqlLearningSpaceRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningSpaceRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlLearningSpaceRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001 and Infrastructure-002: Verify that the repository successfully adds
    /// a learning space to the database and calls SaveChanges.
    /// </summary>
    [Test]
    [Description("Infrastructure-001 and Infrastructure-002: Verify repository adds learning space and calls SaveChanges")]
    public async Task AddAsync_ValidLearningSpace_CallsAddAndSaveChanges()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        _mockDbContext
            .Setup(c => c.LearningSpaces)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await _sut.AddAsync(learningSpace);

        // Assert
        mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls =>
            ls.Type == "Classroom" && ls.Height == 3.0f)), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-003: Verify that the repository propagates database exceptions when SaveChanges fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify repository propagates database exceptions")]
    public async Task AddAsync_SaveChangesFails_ThrowsDbUpdateException()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        _mockDbContext
            .Setup(c => c.LearningSpaces)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));

        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        DbUpdateException? caughtException = null;
        try
        {
            await _sut.AddAsync(learningSpace);
        }
        catch (DbUpdateException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DbUpdateException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Database error"));
        });
    }

    /// <summary>
    /// Infrastructure-004: Verify that the repository handles multiple learning spaces.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify repository handles multiple learning spaces")]
    public async Task AddAsync_MultipleSpaces_CallsAddForEach()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        _mockDbContext
            .Setup(c => c.LearningSpaces)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var space1 = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);
        var space2 = new LearningSpace("Laboratory", 3.5f, 12.0f, 15.0f);

        // Act
        await _sut.AddAsync(space1);
        await _sut.AddAsync(space2);

        // Assert
        mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls => ls.Type == "Classroom")), Times.Once);
        mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls => ls.Type == "Laboratory")), Times.Once);
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
