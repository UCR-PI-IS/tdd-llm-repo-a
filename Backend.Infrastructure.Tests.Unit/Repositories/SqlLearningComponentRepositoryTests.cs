using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockContext = null!;
    private SqlLearningComponentRepository _sut = null!;

    private const string ValidLearningSpaceId = "IF-0103";
    private const string NonExistentLearningSpaceId = "NON-EXISTENT";

    [SetUp]
    public void SetUp()
    {
        var mockOptions = new Mock<DbContextOptions<UCRDatabaseContext>>();
        _mockContext = new Mock<UCRDatabaseContext>(mockOptions.Object);
        _sut = new SqlLearningComponentRepository(_mockContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockContext = null!;
        _sut = null!;
    }

    /// <summary>
    /// Verifies the repository returns a list of components for a valid learning space ID.
    /// </summary>
    [Test(Description = "Infrastructure-001: Returns components for a valid learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 2.0f, 0.0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 1.0f, 1.0f, 0.3f, 3.0f, 4.0f, 0.0f, "South")
        };

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Verifies the repository returns an empty list when the learning space has no components.
    /// </summary>
    [Test(Description = "Infrastructure-002: Returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var emptyComponents = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(emptyComponents.AsQueryable());
        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies the repository returns an empty list when the learning space ID does not exist.
    /// </summary>
    [Test(Description = "Infrastructure-003: Returns empty list for non-existent learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentId = NonExistentLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", ValidLearningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 2.0f, 0.0f, "North")
        };

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(nonExistentId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{TEntity}"/> that supports synchronous LINQ queries.
    /// </summary>
    /// <param name="data">The queryable data source to back the mock DbSet.</param>
    /// <returns>A configured mock DbSet.</returns>
    private static Mock<DbSet<LearningComponent>> CreateMockDbSet(IQueryable<LearningComponent> data)
    {
        var mockSet = new Mock<DbSet<LearningComponent>>();

        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(data.Provider));

        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);

        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);

        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => data.GetEnumerator());

        mockSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(data.GetEnumerator()));

        return mockSet;
    }
}

#region Async Query Helpers for EF Core Mocking

/// <summary>
/// Async query provider that wraps a synchronous <see cref="IQueryProvider"/>
/// to support EF Core async extension methods in unit tests.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider, IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TEntity>(expression);
    }

    public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
    {
        return new TestAsyncEnumerable<TElement>(expression);
    }

    public object? Execute(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: [typeof(System.Linq.Expressions.Expression)])!
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, [expression]);

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, [executionResult])!;
    }
}

/// <summary>
/// Marker interface for async query providers used by EF Core.
/// </summary>
internal interface IAsyncQueryProvider : IQueryProvider
{
    TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default);
}

/// <summary>
/// An <see cref="IQueryable{T}"/> implementation that supports async enumeration
/// for use in EF Core unit tests.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(System.Linq.Expressions.Expression expression)
        : base(expression)
    {
    }

    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider
    {
        get { return new TestAsyncQueryProvider<T>(this); }
    }
}

/// <summary>
/// An <see cref="IAsyncEnumerator{T}"/> wrapper around a synchronous <see cref="IEnumerator{T}"/>
/// for use in EF Core unit tests.
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

#endregion
