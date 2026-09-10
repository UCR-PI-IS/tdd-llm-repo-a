using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/> write operations
/// (<see cref="SqlLearningComponentRepository.ExistsAsync"/> and
/// <see cref="SqlLearningComponentRepository.AddAsync"/>).
/// Covers intents Infrastructure-002 through Infrastructure-005 from story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryWriteTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _sut = null!;

    // Valid test data
    private const string ExistingComponentId = "COMP-12345";
    private const string NonExistentComponentId = "COMP-NONEXISTENT";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _sut = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking existence returns true
    /// when a component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify that ExistsAsync returns true when a component with the given ID exists")]
    public async Task ExistsAsync_ComponentExists_ReturnsTrue()
    {
        // Arrange
        var data = new List<LearningComponent>
        {
            new LearningComponent(ExistingComponentId, "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North")
        }.AsQueryable();

        var mockDbSet = CreateMockDbSet(data);
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsAsync(ExistingComponentId);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking existence returns false
    /// when a component with the given ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify that ExistsAsync returns false when a component with the given ID does not exist")]
    public async Task ExistsAsync_ComponentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var data = new List<LearningComponent>().AsQueryable();

        var mockDbSet = CreateMockDbSet(data);
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsAsync(NonExistentComponentId);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that adding a component with a unique ID
    /// succeeds and calls SaveChanges on the database context.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify that AddAsync succeeds and calls SaveChanges")]
    public async Task AddAsync_UniqueComponent_SucceedsAndCallsSaveChanges()
    {
        // Arrange
        var component = new LearningComponent(
            "COMP-NEW", "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");

        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(_mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.AddAsync(component);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(
                x => x.AddAsync(component, It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDbContext.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-005: Verify that adding a component with a duplicate ID
    /// throws a DbUpdateException due to unique constraint violation.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify that AddAsync throws DbUpdateException for duplicate ID")]
    public async Task AddAsync_DuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent(
            "COMP-DUPLICATE", "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");

        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(_mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act
        DbUpdateException? caughtException = null;
        try
        {
            await _sut.AddAsync(component);
        }
        catch (DbUpdateException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DbUpdateException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Duplicate key"));
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
