using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="ComponentIdGenerator.GenerateIdAsync"/> and
/// <see cref="SqlLearningComponentRepository"/> ExistsAsync and AddAsync methods.
/// Covers intents Infrastructure-001 through Infrastructure-005 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentInfrastructureTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    // Valid test data
    private const string ExistingComponentId = "COMP-12345";
    private const string NonExistentComponentId = "COMP-NONEXISTENT";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify that the ID generator produces a unique ID in the correct format")]
    public async Task GenerateIdAsync_ProducesCorrectFormat()
    {
        // Arrange
        var idGenerator = new ComponentIdGenerator();

        // Act
        var generatedId = await idGenerator.GenerateIdAsync();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(generatedId, Is.Not.Null);
            Assert.That(generatedId, Does.StartWith("COMP-"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify that ExistsAsync returns true when component exists")]
    public async Task ExistsAsync_ComponentExists_ReturnsTrue()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent(ExistingComponentId, "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North")
        };
        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var exists = await _repository.ExistsAsync(ExistingComponentId);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify that ExistsAsync returns false when component does not exist")]
    public async Task ExistsAsync_ComponentDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var components = new List<LearningComponent>();
        var mockDbSet = CreateMockDbSet(components.AsQueryable());
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);

        // Act
        var exists = await _repository.ExistsAsync(NonExistentComponentId);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify that AddAsync succeeds with unique ID and calls SaveChanges")]
    public async Task AddAsync_UniqueId_SucceedsAndCallsSaveChanges()
    {
        // Arrange
        var component = new LearningComponent(ExistingComponentId, "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");
        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        await _repository.AddAsync(component);

        // Assert
        mockDbSet.Verify(x => x.AddAsync(component, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-005: Verify that adding a component with a duplicate ID throws a DbUpdateException due to unique constraint violation.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify that AddAsync throws DbUpdateException for duplicate ID")]
    public void AddAsync_DuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent(ExistingComponentId, "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");
        var mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(component));
        Assert.That(ex!.Message, Does.Contain("Duplicate key"));
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
