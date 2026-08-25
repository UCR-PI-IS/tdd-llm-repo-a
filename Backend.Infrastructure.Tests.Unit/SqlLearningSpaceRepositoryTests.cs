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
    private Mock<DbSet<LearningSpace>> _mockDbSet = null!;
    private SqlLearningSpaceRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningSpace>>();
        _mockDbContext.Setup(c => c.LearningSpaces).Returns(_mockDbSet.Object);
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _sut = new SqlLearningSpaceRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that the repository successfully adds a learning space
    /// to the DbSet with the correct entity properties.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify repository adds a learning space to the database with correct properties")]
    public async Task AddAsync_ValidLearningSpace_CallsAddOnDbSet()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await _sut.AddAsync(learningSpace);

        // Assert
        _mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls =>
            ls.Type == "Classroom" && ls.Height == 3.0f && ls.Width == 8.0f && ls.Length == 10.0f)),
            Times.Once);
    }

    /// <summary>
    /// Infrastructure-002: Verify that the repository calls SaveChangesAsync to persist
    /// the learning space to the database after adding it.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify repository calls SaveChanges to persist the learning space")]
    public async Task AddAsync_ValidLearningSpace_CallsSaveChanges()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await _sut.AddAsync(learningSpace);

        // Assert
        _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-003: Verify that the repository propagates database exceptions
    /// when SaveChangesAsync fails.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify repository propagates database exceptions when SaveChanges fails")]
    public async Task AddAsync_SaveChangesFails_ThrowsDbUpdateException()
    {
        // Arrange
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
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
    /// Infrastructure-004: Verify that the repository correctly adds multiple learning
    /// spaces when called sequentially, invoking Add for each entity.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify repository adds multiple learning spaces sequentially")]
    public async Task AddAsync_MultipleLearningSpaces_CallsAddForEach()
    {
        // Arrange
        var space1 = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);
        var space2 = new LearningSpace("Laboratory", 3.5f, 12.0f, 15.0f);

        // Act
        await _sut.AddAsync(space1);
        await _sut.AddAsync(space2);

        // Assert
        _mockDbSet.Verify(d => d.Add(It.IsAny<LearningSpace>()), Times.Exactly(2));
    }
}
