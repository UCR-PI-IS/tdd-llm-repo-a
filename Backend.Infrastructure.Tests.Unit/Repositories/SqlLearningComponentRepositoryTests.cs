using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext;
    private SqlLearningComponentRepository _repository;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync returns a list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithValidId_ReturnsListOfComponents()
    {
        // Arrange
        string learningSpaceId = "IF-0103";
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "component-001",
                learningSpaceId,
                2.5f,
                1.5f,
                1.0f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "component-002",
                learningSpaceId,
                3.0f,
                2.0f,
                1.5f,
                15.0f,
                10.0f,
                0.0f,
                "South")
        };

        var mockDbSet = CreateMockDbSet(expectedComponents.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

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
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        string learningSpaceId = "IF-0202";
        var emptyComponents = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(emptyComponents.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

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

    /// <summary>
    /// Tests that GetComponentsByLearningSpaceIdAsync returns an empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "IF-9999";
        var existingComponents = new List<LearningComponent>
        {
            new LearningComponent(
                "component-001",
                "IF-0103",
                2.5f,
                1.5f,
                1.0f,
                10.0f,
                5.0f,
                0.0f,
                "North")
        };

        var mockDbSet = CreateMockDbSet(existingComponents.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

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

    /// <summary>
    /// Creates a mock DbSet from a queryable collection.
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(data.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}

/// <summary>
/// Test helper class for async enumeration support in mocked DbSets.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public T Current => _inner.Current;
}

/// <summary>
/// Test helper class for async query provider support in mocked DbSets.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: new[] { typeof(Expression) })
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, new[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { executionResult });
    }
}

/// <summary>
/// Test helper class for async enumerable support in mocked DbSets.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}
