using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> infrastructure repository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext;
    private SqlLearningComponentRepository _repository;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>().Options;
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidId_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", learningSpaceId, 2.0f, 3.0f, 1.5f, 0.0f, 0.0f, 0.0f, Orientation.North),
            new LearningComponent("comp-002", learningSpaceId, 1.0f, 2.0f, 1.0f, 1.0f, 0.0f, 0.0f, Orientation.South)
        };

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

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

    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>();

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

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

    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "space-999";
        var components = new List<LearningComponent>();

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

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

    private static Mock<DbSet<LearningComponent>> CreateMockDbSet(List<LearningComponent> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<LearningComponent>>();

        mockSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<LearningComponent>(queryable.Provider));
        mockSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return mockSet;
    }
}

/// <summary>
/// Async query provider for mocking EF Core <see cref="DbSet{T}"/> operations.
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

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Execute<TResult>(expression);
    }
}

/// <summary>
/// Async enumerable for mocking EF Core <see cref="DbSet{T}"/> operations.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

/// <summary>
/// Async enumerator for mocking EF Core <see cref="DbSet{T}"/> operations.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}
