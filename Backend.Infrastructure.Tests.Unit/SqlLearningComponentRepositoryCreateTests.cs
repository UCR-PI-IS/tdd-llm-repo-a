using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/> create operations.
/// Covers intents Infrastructure-002 through Infrastructure-005 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryCreateTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _sut = null!;

    // Valid test data
    private const string ValidComponentId = "COMP-12345";
    private const string ValidLearningSpaceId = "LS-001";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _sut = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists")]
    public async Task ExistsAsync_ComponentExists_ReturnsTrue()
    {
        // Arrange
        var componentId = ValidComponentId;
        var components = new List<LearningComponent>
        {
            new LearningComponent(componentId, ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North")
        };

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsAsync(componentId);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist")]
    public async Task ExistsAsync_ComponentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = "COMP-NONEXISTENT";
        var components = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsAsync(nonExistentId);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges")]
    public async Task AddAsync_UniqueId_CallsAddAndSaveChanges()
    {
        // Arrange
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        _mockDbContext
            .Setup(x => x.LearningComponents)
            .Returns(_mockDbSet.Object);
        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.AddAsync(component);

        // Assert
        _mockDbSet.Verify(x => x.AddAsync(component, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-005: Verify that adding a component with a duplicate ID throws a DbUpdateException due to unique constraint violation.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify that adding a component with a duplicate ID throws a DbUpdateException")]
    public void AddAsync_DuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        _mockDbContext
            .Setup(x => x.LearningComponents)
            .Returns(_mockDbSet.Object);
        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act & Assert
        DbUpdateException? caughtException = null;
        try
        {
            _sut.AddAsync(component).Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is DbUpdateException)
        {
            caughtException = ex.InnerException as DbUpdateException;
        }

        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected DbUpdateException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Duplicate key"));
        });
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> that supports synchronous and asynchronous LINQ operations.
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
    /// Async query provider for EF Core async operations.
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

    private interface IAsyncQueryProvider : IQueryProvider
    {
        TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default);
    }

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
