using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningSpaceRepository.
/// </summary>
[TestFixture]
public class SqlLearningSpaceRepositoryTests
{
    private DbContextOptions<UCRDatabaseContext> _dbContextOptions = null!;

    /// <summary>
    /// Sets up the test fixture before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _dbContextOptions = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    /// <summary>
    /// Verifies that the repository successfully adds a learning space to the database.
    /// </summary>
    [Test]
    [Description("Adds a learning space to the database and verifies persistence")]
    public async Task AddAsync_WithValidLearningSpace_PersistsToDatabase()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_dbContextOptions);
        var repository = new SqlLearningSpaceRepository(context);
        var learningSpace = new LearningSpace("IF-0101", "Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await repository.AddAsync(learningSpace);

        // Assert
        var savedSpace = await context.LearningSpaces.FirstOrDefaultAsync();
        Assert.Multiple(() =>
        {
            Assert.That(savedSpace, Is.Not.Null);
            Assert.That(savedSpace!.id, Is.EqualTo("IF-0101"));
            Assert.That(savedSpace.type, Is.EqualTo("Classroom"));
            Assert.That(savedSpace.height, Is.EqualTo(3.0f));
            Assert.That(savedSpace.width, Is.EqualTo(8.0f));
            Assert.That(savedSpace.length, Is.EqualTo(10.0f));
        });
    }

    /// <summary>
    /// Verifies that the repository calls SaveChanges to persist the learning space.
    /// </summary>
    [Test]
    [Description("Verifies that AddAsync calls SaveChangesAsync on the context")]
    public async Task AddAsync_CallsSaveChangesAsync()
    {
        // Arrange
        var mockContext = new Mock<UCRDatabaseContext>(new DbContextOptions<UCRDatabaseContext>());
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        
        mockContext.Setup(c => c.LearningSpaces).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        var repository = new SqlLearningSpaceRepository(mockContext.Object);
        var learningSpace = new LearningSpace("IF-0101", "Classroom", 3.0f, 8.0f, 10.0f);

        // Act
        await repository.AddAsync(learningSpace);

        // Assert
        mockDbSet.Verify(d => d.Add(It.Is<LearningSpace>(ls =>
            ls.id == "IF-0101" && ls.type == "Classroom" && ls.height == 3.0f)), Times.Once);
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that the repository propagates database exceptions when SaveChanges fails.
    /// </summary>
    [Test]
    [Description("Validates that DbUpdateException is propagated when SaveChangesAsync fails")]
    public void AddAsync_WhenSaveChangesFails_ThrowsDbUpdateException()
    {
        // Arrange
        var mockContext = new Mock<UCRDatabaseContext>(new DbContextOptions<UCRDatabaseContext>());
        var mockDbSet = new Mock<DbSet<LearningSpace>>();
        
        mockContext.Setup(c => c.LearningSpaces).Returns(mockDbSet.Object);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Database error"));
        
        var repository = new SqlLearningSpaceRepository(mockContext.Object);
        var learningSpace = new LearningSpace("IF-0101", "Classroom", 3.0f, 8.0f, 10.0f);

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
            await repository.AddAsync(learningSpace));
        Assert.That(ex.Message, Does.Contain("Database error"));
    }

    /// <summary>
    /// Verifies that multiple learning spaces can be added with unique identifiers.
    /// </summary>
    [Test]
    [Description("Adds multiple learning spaces and verifies they are all persisted")]
    public async Task AddAsync_MultipleLearningSpaces_PersistsAll()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_dbContextOptions);
        var repository = new SqlLearningSpaceRepository(context);
        var space1 = new LearningSpace("IF-0101", "Classroom", 3.0f, 8.0f, 10.0f);
        var space2 = new LearningSpace("IF-0201", "Laboratory", 3.5f, 12.0f, 15.0f);

        // Act
        await repository.AddAsync(space1);
        await repository.AddAsync(space2);

        // Assert
        var spaces = await context.LearningSpaces.OrderBy(ls => ls.id).ToListAsync();
        Assert.Multiple(() =>
        {
            Assert.That(spaces.Count, Is.EqualTo(2));
            Assert.That(spaces[0].id, Is.EqualTo("IF-0101"));
            Assert.That(spaces[1].id, Is.EqualTo("IF-0201"));
        });
    }
}
