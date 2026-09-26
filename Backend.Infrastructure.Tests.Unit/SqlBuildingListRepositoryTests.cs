using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlBuildingListRepository.GetAllBuildingsAsync"/>.
/// Covers intents Infrastructure-001 through Infrastructure-002.
/// </summary>
[TestFixture]
public class SqlBuildingListRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlBuildingListRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlBuildingListRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify repository returns all buildings from database context when buildings exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository returns all buildings when database has data")]
    public async Task GetAllBuildingsAsync_BuildingsExist_ReturnsAllBuildings()
    {
        // Arrange
        var expectedBuildings = new List<Building>
        {
            new Building(1, "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f),
            new Building(2, "Science Building", "Blue", 25.0f, 60.0f, 40.0f, 150.0f, 250.0f, 0.0f)
        };

        var mockDbSet = CreateMockDbSet(expectedBuildings.AsQueryable());
        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Engineering Building"));
            Assert.That(result[1].Name, Is.EqualTo("Science Building"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify repository returns empty list when database contains no buildings.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns empty list when database has no buildings")]
    public async Task GetAllBuildingsAsync_NoBuildingsInDatabase_ReturnsEmptyList()
    {
        // Arrange
        var emptyBuildings = new List<Building>().AsQueryable();

        var mockDbSet = CreateMockDbSet(emptyBuildings);
        _mockDbContext
            .Setup(c => c.Buildings)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetAllBuildingsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Has.Count.EqualTo(0));
        });
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> that supports synchronous and asynchronous
    /// LINQ operations for the given queryable data.
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(IQueryable<T> data) where T : class
    {
        var mockDbSet = new Mock<DbSet<T>>();

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

        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));

        return mockDbSet;
    }

    private class AsyncQueryProvider<TEntity> : IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
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

    private class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(Expression expression) : base(expression)
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
