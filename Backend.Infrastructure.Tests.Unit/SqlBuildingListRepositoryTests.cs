using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingListRepository.GetAllBuildingsAsync"/>.
/// Covers intents Infrastructure-001 and Infrastructure-002.
/// </summary>
[TestFixture]
public class SqlBuildingListRepositoryTests
{
    /// <summary>
    /// Infrastructure-001: Verify repository returns all buildings from database context when buildings exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository returns all buildings from database when buildings exist")]
    public async Task GetAllBuildingsAsync_BuildingsExist_ReturnsAllBuildings()
    {
        // Arrange
        var expectedBuildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        }.AsQueryable();

        var mockDbSet = CreateMockDbSet(expectedBuildings);
        var options = new DbContextOptions<UCRDatabaseContext>();
        var mockDbContext = new Mock<UCRDatabaseContext>(options);
        mockDbContext.Setup(c => c.Buildings).Returns(mockDbSet.Object);

        var sut = new SqlBuildingListRepository(mockDbContext.Object);

        // Act
        var result = await sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Engineering Building"));
            Assert.That(result[1].Name, Is.EqualTo("Science Building"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify repository returns empty list when database contains no buildings.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns empty list when database contains no buildings")]
    public async Task GetAllBuildingsAsync_NoBuildings_ReturnsEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>().AsQueryable();

        var mockDbSet = CreateMockDbSet(emptyBuildings);
        var options = new DbContextOptions<UCRDatabaseContext>();
        var mockDbContext = new Mock<UCRDatabaseContext>(options);
        mockDbContext.Setup(c => c.Buildings).Returns(mockDbSet.Object);

        var sut = new SqlBuildingListRepository(mockDbContext.Object);

        // Act
        var result = await sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result.Count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Creates a fully configured Mock&lt;DbSet&lt;T&gt;&gt; with IQueryable and IAsyncEnumerable support.
    /// All As&lt;T&gt;() calls are made before .Object is accessed, preventing Moq initialization errors.
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockDbSet = new Mock<DbSet<T>>();

        // Set up IQueryable<T> members
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new AsyncQueryProvider<T>(data.Provider));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => data.GetEnumerator());

        // Set up IAsyncEnumerable<T> members
        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));

        return mockDbSet;
    }

    /// <summary>
    /// Async query provider that wraps a synchronous <see cref="IQueryProvider"/>
    /// to support EF Core async LINQ extension methods.
    /// </summary>
    private class AsyncQueryProvider<TEntity> : IQueryProvider, IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
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
    /// Marker interface for async query providers used by EF Core async extension methods.
    /// </summary>
    private interface IAsyncQueryProvider : IQueryProvider
    {
        TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Async enumerable wrapper that enables EF Core async LINQ operations on in-memory data.
    /// </summary>
    private class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(System.Linq.Expressions.Expression expression) : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new AsyncQueryProvider<T>(this); }
        }
    }

    /// <summary>
    /// Async enumerator wrapper that enables EF Core async iteration on synchronous enumerators.
    /// </summary>
    private class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
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
}
