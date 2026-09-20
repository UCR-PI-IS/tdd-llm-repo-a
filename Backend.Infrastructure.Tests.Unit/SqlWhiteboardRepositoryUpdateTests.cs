using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlWhiteboardRepository"/> update and read operations.
/// Covers intents Infrastructure-001 through Infrastructure-004 for CPD-LC-001-005.
/// </summary>
[TestFixture]
public class SqlWhiteboardRepositoryUpdateTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlWhiteboardRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _sut = new SqlWhiteboardRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that repository successfully retrieves a whiteboard by ID from the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository successfully retrieves a whiteboard by ID from the database")]
    public async Task GetByIdAsync_ExistingWhiteboard_ReturnsWhiteboard()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "LS-001";
        var expectedWhiteboard = new Whiteboard(
            whiteboardId, learningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        mockDbSet.Setup(d => d.FindAsync(whiteboardId)).ReturnsAsync(expectedWhiteboard);

        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByIdAsync(whiteboardId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.ComponentId, Is.EqualTo(whiteboardId));
            Assert.That(result.MarkerColor, Is.EqualTo("Blue"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that repository returns null when whiteboard with given ID does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns null when whiteboard with given ID does not exist")]
    public async Task GetByIdAsync_NonExistentWhiteboard_ReturnsNull()
    {
        // Arrange
        var nonExistentId = "WB-999";
        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        mockDbSet.Setup(d => d.FindAsync(nonExistentId)).ReturnsAsync((Whiteboard?)null);

        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByIdAsync(nonExistentId);

        // Assert
        Assert.That(result, Is.Null);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository successfully updates a whiteboard in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository successfully updates a whiteboard in the database")]
    public async Task UpdateAsync_ValidWhiteboard_UpdatesAndSaves()
    {
        // Arrange
        var whiteboardId = "WB-001";
        var learningSpaceId = "LS-001";
        var whiteboardToUpdate = new Whiteboard(
            whiteboardId, learningSpaceId,
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        var mockDbSet = new Mock<DbSet<Whiteboard>>();
        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.UpdateAsync(whiteboardToUpdate);

        // Assert
        Assert.Multiple(() =>
        {
            mockDbSet.Verify(
                d => d.Update(It.Is<Whiteboard>(w =>
                    w.ComponentId == whiteboardId && w.MarkerColor == "Red")),
                Times.Once);
            _mockDbContext.Verify(
                c => c.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-004: Verify that repository retrieves all whiteboards in a learning space.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Repository retrieves all whiteboards in a learning space")]
    public async Task GetByLearningSpaceIdAsync_WhiteboardsInSpace_ReturnsList()
    {
        // Arrange
        var learningSpaceId = "LS-001";
        var whiteboard1 = new Whiteboard(
            "WB-001", learningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");
        var whiteboard2 = new Whiteboard(
            "WB-002", learningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "South", "Green");

        var whiteboards = new List<Whiteboard> { whiteboard1, whiteboard2 }.AsQueryable();
        var mockDbSet = CreateMockDbSet(whiteboards);

        _mockDbContext
            .Setup(c => c.Whiteboards)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.GetByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
        });
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
