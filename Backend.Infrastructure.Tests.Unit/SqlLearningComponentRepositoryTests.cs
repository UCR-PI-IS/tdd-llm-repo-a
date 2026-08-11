using Moq;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.TestHelpers;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// Tests the repository's data access logic using mocked EF Core DbSet.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    private const string ValidLearningSpaceId = "ls-001";
    private const string EmptyLearningSpaceId = "ls-002";
    private const string NonExistentLearningSpaceId = "ls-999";

    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        
        // Configure all mock interfaces BEFORE accessing .Object
        // This is required by Moq - .As<>() must be called before .Object is accessed
        SetupMockDbSet(new List<LearningComponent>().AsQueryable());
        
        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify repository returns a list of components for a valid
    /// learning space ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Returns list of components for a valid learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsComponentList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", ValidLearningSpaceId, 2f, 3f, 1.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("comp-002", ValidLearningSpaceId, 1f, 2f, 1f, 5f, 10f, 0f, "South")
        };

        SetupMockDbSet(components.AsQueryable());

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == ValidLearningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify repository returns an empty list when the learning space
    /// has no components in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var allComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", ValidLearningSpaceId, 2f, 3f, 1.5f, 10f, 20f, 0f, "North")
        };

        SetupMockDbSet(allComponents.AsQueryable());

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(EmptyLearningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Infrastructure-003: Verify repository returns an empty list when the learning space ID
    /// does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Returns empty list when learning space ID does not exist")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        var allComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", ValidLearningSpaceId, 2f, 3f, 1.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("comp-002", ValidLearningSpaceId, 1f, 2f, 1f, 5f, 10f, 0f, "South")
        };

        SetupMockDbSet(allComponents.AsQueryable());

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(NonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Configures the mock <see cref="DbSet{TEntity}"/> to behave as a queryable collection
    /// with async enumeration support, enabling EF Core LINQ operations like Where + ToListAsync.
    /// </summary>
    /// <param name="data">The data to back the mock DbSet.</param>
    private void SetupMockDbSet(IQueryable<LearningComponent> data)
    {
        var asyncProvider = new TestAsyncQueryProvider<LearningComponent>(data.Provider);

        _mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(asyncProvider);
        _mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => data.GetEnumerator());
        _mockDbSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(data.AsEnumerable().GetEnumerator()));
    }
}
