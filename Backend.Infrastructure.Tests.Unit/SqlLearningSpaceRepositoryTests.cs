using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningSpaceRepository.
/// </summary>
[TestFixture]
public class SqlLearningSpaceRepositoryTests
{
    /// <summary>
    /// Verifies that the repository successfully adds a learning space to the database 
    /// and assigns a sequential internal ID.
    /// </summary>
    [Test]
    [Description("Verify that the repository successfully adds a learning space to the database and assigns a sequential internal ID")]
    public async Task AddAsync_ValidLearningSpace_SavesToDatabase()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        using var context = new UCRDatabaseContext(options);
        var repository = new SqlLearningSpaceRepository(context);
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await repository.AddAsync(learningSpace);

        // Assert
        var savedSpace = await context.LearningSpaces.FirstOrDefaultAsync();
        Assert.Multiple(() =>
        {
            Assert.That(savedSpace, Is.Not.Null);
            Assert.That(savedSpace!.LearningSpaceId, Is.GreaterThan(0));
            Assert.That(savedSpace.Type, Is.EqualTo("Classroom"));
        });
    }

    /// <summary>
    /// Verifies that the repository calls SaveChanges to persist the learning space to the database.
    /// </summary>
    [Test]
    [Description("Verify that the repository calls SaveChanges to persist the learning space to the database")]
    public async Task AddAsync_ValidLearningSpace_CallsSaveChanges()
    {
        // Arrange
        var mockContext = new Mock<UCRDatabaseContext>();
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        mockContext.Setup(c => c.LearningSpaces).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        var repository = new SqlLearningSpaceRepository(mockContext.Object);
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await repository.AddAsync(learningSpace);

        // Assert
        mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls => 
            ls.Type == "Classroom" && ls.Height == 3.0f)), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that the repository propagates database exceptions when SaveChanges fails.
    /// </summary>
    [Test]
    [Description("Verify that the repository propagates database exceptions when SaveChanges fails")]
    public void AddAsync_DatabaseException_PropagatesException()
    {
        // Arrange
        var mockContext = new Mock<UCRDatabaseContext>();
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        mockContext.Setup(c => c.LearningSpaces).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));
        var repository = new SqlLearningSpaceRepository(mockContext.Object);
        var learningSpace = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(async () => 
            await repository.AddAsync(learningSpace));
        Assert.That(ex.Message, Does.Contain("Database error"));
    }

    /// <summary>
    /// Verifies that sequential internal IDs are generated correctly when adding multiple learning spaces.
    /// </summary>
    [Test]
    [Description("Verify that sequential internal IDs are generated correctly when adding multiple learning spaces")]
    public async Task AddAsync_MultipleLearningSpaces_GeneratesSequentialIds()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Sequential")
            .Options;
        using var context = new UCRDatabaseContext(options);
        var repository = new SqlLearningSpaceRepository(context);
        var space1 = new LearningSpace("Classroom", 3.0f, 8.0f, 10.0f);
        var space2 = new LearningSpace("Laboratory", 3.5f, 12.0f, 15.0f);

        // Act
        await repository.AddAsync(space1);
        await repository.AddAsync(space2);

        // Assert
        var spaces = await context.LearningSpaces.OrderBy(ls => ls.LearningSpaceId).ToListAsync();
        Assert.That(spaces, Has.Count.EqualTo(2));
        Assert.That(spaces[1].LearningSpaceId, Is.GreaterThan(spaces[0].LearningSpaceId));
    }
}
