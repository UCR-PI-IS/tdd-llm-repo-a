using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/>.
/// Covers GetComponentsByLearningSpaceIdAsync (original intents) and
/// ExistsAsync / AddAsync (CPD-LC-001-009 intents Infrastructure-002 through Infrastructure-005).
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _sut = null!;

    // Valid test data
    private const string ValidLearningSpaceId = "IF-0103";
    private const string NonExistentLearningSpaceId = "NON-EXISTENT-999";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify repository returns list of components for a valid
    /// learning space ID from database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify repository returns list of components for a valid learning space ID")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("LC-001", learningSpaceId, 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North"),
            new LearningComponent("LC-002", learningSpaceId, 3.0f, 2.0f, 1.0f, 15f, 25f, 0f, "South")
        };

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext
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
    /// Infrastructure-002 and Infrastructure-003: Verify repository returns empty list
    /// when learning space has no components or when learning space ID does not exist.
    /// </summary>
    [TestCase(ValidLearningSpaceId,
        Description = "Infrastructure-002: Returns empty list when learning space has no components")]
    [TestCase(NonExistentLearningSpaceId,
        Description = "Infrastructure-003: Returns empty list when learning space ID does not exist")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoMatchingComponents_ReturnsEmptyList(string learningSpaceId)
    {
        // Arrange
        var allComponents = new List<LearningComponent>
        {
            new LearningComponent("LC-001", "OTHER-SPACE", 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North")
        };

        var mockDbSet = CreateMockDbSet(allComponents.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Has.Count.EqualTo(0));
        });
    }

    // ──────────────────────────────────────────────────────────────────────────
    // ExistsAsync tests (CPD-LC-001-009: Infrastructure-002 and Infrastructure-003)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Infrastructure-002 (CPD-LC-001-009): Verify that ExistsAsync returns true when
    /// a component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsAsync returns true when component ID exists")]
    public async Task ExistsAsync_ComponentExists_ReturnsTrue()
    {
        // Arrange
        var existingId = "COMP-12345";
        var components = new List<LearningComponent>
        {
            new LearningComponent(existingId, "LS-001", 2.5f, 1.5f, 0.5f, 10f, 20f, 0f, "North")
        };

        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsAsync(existingId);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003 (CPD-LC-001-009): Verify that ExistsAsync returns false when
    /// no component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsAsync returns false when component ID does not exist")]
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

    // ──────────────────────────────────────────────────────────────────────────
    // AddAsync tests (CPD-LC-001-009: Infrastructure-004 and Infrastructure-005)
    // ──────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Infrastructure-004 (CPD-LC-001-009): Verify that AddAsync adds the component
    /// to the DbSet and calls SaveChangesAsync on the context.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: AddAsync adds component and calls SaveChangesAsync")]
    public async Task AddAsync_UniqueComponent_SucceedsAndCallsSaveChanges()
    {
        // Arrange
        var component = new LearningComponent(
            "COMP-NEW", "LS-001", 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.AddAsync(component);

        // Assert
        mockDbSet.Verify(
            x => x.AddAsync(component, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Infrastructure-005 (CPD-LC-001-009): Verify that AddAsync propagates a
    /// DbUpdateException when SaveChangesAsync fails due to a unique constraint violation.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: AddAsync throws DbUpdateException on duplicate key violation")]
    public async Task AddAsync_DuplicateComponent_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent(
            "COMP-DUP", "LS-001", 1.5f, 1.0f, 0.5f, 10f, 5f, 0f, "North");

        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act & Assert
        var caughtException = Assert.ThrowsAsync<DbUpdateException>(
            () => _sut.AddAsync(component));

        Assert.That(caughtException!.Message, Does.Contain("Duplicate key"));
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> that supports synchronous and asynchronous
    /// LINQ operations for the given queryable data.
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
