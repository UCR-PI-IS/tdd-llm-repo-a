using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.TestHelpers;

/// <summary>
/// Provides helper methods for creating mocked <see cref="DbSet{T}"/> instances
/// that support both synchronous LINQ queries and EF Core async operations (e.g., ToListAsync).
/// </summary>
internal static class MockDbSetHelper
{
    /// <summary>
    /// Creates a <see cref="Mock{DbSet}"/> backed by the provided in-memory list.
    /// The mock supports IQueryable (for Where, Select, etc.) and IAsyncEnumerable (for ToListAsync).
    /// </summary>
    /// <typeparam name="T">The entity type of the DbSet.</typeparam>
    /// <param name="data">The in-memory data to back the mock DbSet.</param>
    /// <returns>A configured <see cref="Mock{DbSet}"/> instance.</returns>
    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        // Set up IAsyncEnumerable<T> so that EF Core's ToListAsync works
        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

        // Set up IQueryable<T> so that LINQ operators (Where, Select, etc.) work
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

        return mockSet;
    }
}

/// <summary>
/// A query provider that creates async-aware queryable instances,
/// enabling EF Core async extension methods (e.g., ToListAsync) to work with mocked DbSets.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetGenericArguments()[0];
        var queryableType = typeof(TestAsyncEnumerable<>).MakeGenericType(elementType);
        return (IQueryable)Activator.CreateInstance(queryableType, expression)!;
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
}

/// <summary>
/// An <see cref="EnumerableQuery{T}"/> that also implements <see cref="IAsyncEnumerable{T}"/>,
/// allowing EF Core async operations to work on in-memory data during unit tests.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider
    {
        get { return new TestAsyncQueryProvider<T>(((IQueryable)this).Provider); }
    }
}

/// <summary>
/// Wraps a synchronous <see cref="IEnumerator{T}"/> as an <see cref="IAsyncEnumerator{T}"/>
/// for use in mocked async enumeration during unit tests.
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
