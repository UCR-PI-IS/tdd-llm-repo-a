using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Helper utilities for creating mock <see cref="DbSet{T}"/> instances
/// that support async LINQ operations (Where, ToListAsync) via Moq.
/// </summary>
internal static class MockDbSetHelper
{
    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> backed by the provided in-memory data,
    /// supporting both synchronous IQueryable and asynchronous IAsyncEnumerable operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The in-memory data to back the mock DbSet.</param>
    /// <returns>A configured <see cref="Mock{T}"/> of <see cref="DbSet{T}"/>.</returns>
    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        return mockSet;
    }
}

/// <summary>
/// An <see cref="IQueryable{T}"/> implementation that wraps an expression tree
/// and supports async enumeration via <see cref="IAsyncEnumerable{T}"/>.
/// Extends <see cref="EnumerableQuery{T}"/> for in-memory expression evaluation.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}

/// <summary>
/// A custom <see cref="IQueryProvider"/> that creates <see cref="TestAsyncEnumerable{T}"/>
/// instances for query operations, enabling async enumeration of LINQ results.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression);
}

/// <summary>
/// An <see cref="IAsyncEnumerator{T}"/> adapter that wraps a synchronous <see cref="IEnumerator{T}"/>.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

    public T Current => _inner.Current;

    public ValueTask<bool> MoveNextAsync()
        => new ValueTask<bool>(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return default;
    }
}
