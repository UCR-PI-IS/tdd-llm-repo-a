using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Async queryable helper classes for mocking EF Core DbSet in unit tests.
/// Enables testing of async LINQ operations (e.g., AnyAsync, FirstOrDefaultAsync)
/// against mocked DbSet instances.
/// </summary>
internal static class TestAsyncHelpers
{
    /// <summary>
    /// Creates a mock <see cref="IQueryable{T}"/> that supports async enumeration,
    /// enabling EF Core async extension methods to work against in-memory data.
    /// </summary>
    internal static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source) where T : class
    {
        return new TestAsyncEnumerable<T>(source);
    }
}

/// <summary>
/// An async query provider that wraps a synchronous <see cref="IQueryProvider"/>
/// and implements <see cref="IAsyncQueryProvider"/> for EF Core async operations.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider, IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression);

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(name: nameof(IQueryProvider.Execute), genericParameterCount: 1, types: [typeof(Expression)])!
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, [expression]);
        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, [executionResult])!;
    }
}

/// <summary>
/// An <see cref="IQueryable{T}"/> implementation that supports async enumeration
/// by wrapping an in-memory <see cref="EnumerableQuery{T}"/>.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider
        => new TestAsyncQueryProvider<T>(this);
}

/// <summary>
/// An <see cref="IAsyncEnumerator{T}"/> implementation that wraps a synchronous
/// <see cref="IEnumerator{T}"/> for async enumeration in tests.
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
        => new(_inner.MoveNext());

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return new ValueTask();
    }
}
