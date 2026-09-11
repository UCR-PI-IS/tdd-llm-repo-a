using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
    private const string ExistingComponentId = "COMP-12345";
    private const string NonExistentComponentId = "COMP-NONEXISTENT";
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
    /// Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify ExistsAsync returns true for existing component")]
    public async Task ExistsAsync_WithExistingComponent_ReturnsTrue()
    {
        // Arrange
        var existingComponent = new LearningComponent(
            ExistingComponentId, ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        var data = new List<LearningComponent> { existingComponent }.AsQueryable();
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
    /// Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify ExistsAsync returns false for non-existent component")]
    public async Task ExistsAsync_WithNonExistentComponent_ReturnsFalse()
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
    /// Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify AddAsync succeeds and calls SaveChanges for unique ID")]
    public async Task AddAsync_WithUniqueId_SucceedsAndCallsSaveChanges()
    {
        // Arrange
        var component = new LearningComponent(
            "COMP-NEW", ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(_mockDbSet.Object);

        _mockDbSet
            .Setup(x => x.AddAsync(component, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EntityEntry<LearningComponent>>(Mock.Of<EntityEntry<LearningComponent>>()));

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
    /// Infrastructure-005: Verify that adding a component with a duplicate ID throws DbUpdateException.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify AddAsync throws DbUpdateException for duplicate ID")]
    public void AddAsync_WithDuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent(
            ExistingComponentId, ValidLearningSpaceId, 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(_mockDbSet.Object);

        _mockDbSet
            .Setup(x => x.AddAsync(component, It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<EntityEntry<LearningComponent>>(Mock.Of<EntityEntry<LearningComponent>>));

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(() => _sut.AddAsync(component));
        Assert.That(ex!.Message, Does.Contain("Duplicate key"));
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> that supports synchronous and asynchronous LINQ operations.
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
    /// Async query provider that wraps a synchronous IQueryProvider.
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
    /// Marker interface for async query providers.
    /// </summary>
    private interface IAsyncQueryProvider : IQueryProvider
    {
        TResult ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Async enumerable wrapper for EF Core async operations.
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
    /// Async enumerator wrapper for synchronous enumerators.
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
