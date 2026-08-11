using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.TestHelpers;

/// <summary>
/// A test async query provider that wraps a standard <see cref="IQueryProvider"/>
/// and adds async execution support via <see cref="IAsyncQueryProvider"/>.
/// Used to mock EF Core <see cref="Microsoft.EntityFrameworkCore.DbSet{TEntity}"/> for unit testing.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider, IAsyncQueryProvider
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

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(
                name: nameof(IQueryProvider.Execute),
                genericParameterCount: 1,
                types: new[] { typeof(Expression) })!
            .MakeGenericMethod(expectedResultType)
            .Invoke(this, new[] { expression });

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, new[] { executionResult })!;
    }
}

/// <summary>
/// An <see cref="EnumerableQuery{T}"/> that also implements <see cref="IAsyncEnumerable{T}"/>
/// and exposes a <see cref="TestAsyncQueryProvider{T}"/> as its query provider.
/// This allows LINQ chains (e.g., Where + ToListAsync) to work on mocked DbSets.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
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

    IQueryProvider IQueryable.Provider
    {
        get { return new TestAsyncQueryProvider<T>(this); }
    }
}

/// <summary>
/// Wraps a synchronous <see cref="IEnumerator{T}"/> as an <see cref="IAsyncEnumerator{T}"/>
/// so that <c>await foreach</c> works on mocked queryable data.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
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
