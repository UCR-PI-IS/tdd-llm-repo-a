using Microsoft.EntityFrameworkCore;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    private const string ValidLearningSpaceId = "LS-001";
    private const string NonExistentLearningSpaceId = "LS-999";

    /// <summary>
    /// Sets up the mock DbContext and repository instance before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Creates a list of LearningComponent entities for testing.
    /// </summary>
    private static List<LearningComponent> CreateTestComponents(string learningSpaceId, int count)
    {
        var components = new List<LearningComponent>();
        for (int i = 1; i <= count; i++)
        {
            components.Add(new LearningComponent(
                $"COMP-{i:D3}",
                learningSpaceId,
                10f + i,
                5f + i,
                8f + i,
                2f + i,
                3f + i,
                1f + i,
                "North"));
        }
        return components;
    }

    #region Positive Tests

    /// <summary>
    /// Verify repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ShouldReturnComponentList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var testComponents = CreateTestComponents(learningSpaceId, 2);
        var queryableComponents = testComponents.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(queryableComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(queryableComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(queryableComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(queryableComponents.GetEnumerator());

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Verify repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ShouldReturnEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var testComponents = new List<LearningComponent>();
        var queryableComponents = testComponents.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(queryableComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(queryableComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(queryableComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(queryableComponents.GetEnumerator());

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    #endregion

    #region Negative Tests

    /// <summary>
    /// Verify repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ShouldReturnEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = NonExistentLearningSpaceId;
        var testComponents = new List<LearningComponent>();
        var queryableComponents = testComponents.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(queryableComponents.Provider);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(queryableComponents.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(queryableComponents.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(queryableComponents.GetEnumerator());

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(_mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    #endregion
}