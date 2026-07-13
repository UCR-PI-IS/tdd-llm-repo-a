using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> class.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    /// <summary>
    /// Sets up the test fixtures before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>(MockBehavior.Loose, new DbContextOptions<UCRDatabaseContext>());
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Creates a mock DbSet from a list of entities for testing.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="data">The list of entities.</param>
    /// <returns>A mock DbSet.</returns>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        mockSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        return mockSet;
    }

    /// <summary>
    /// Verifies that the repository returns a list of components for a valid learning space ID.
    /// </summary>
    [Test]
    [Description("Repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                learningSpaceId,
                2.5f,
                3.0f,
                1.5f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "comp-002",
                learningSpaceId,
                3.0f,
                2.5f,
                2.0f,
                15.0f,
                8.0f,
                0.0f,
                "South")
        };

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Object.LearningComponents = mockSet.Object;

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Verifies that the repository returns an empty list when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "space-002";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                "space-001",
                2.5f,
                3.0f,
                1.5f,
                10.0f,
                5.0f,
                0.0f,
                "North")
        };

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Object.LearningComponents = mockSet.Object;

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies that the repository returns an empty list when the learning space ID does not exist.
    /// </summary>
    [Test]
    [Description("Repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "non-existent-space";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                "space-001",
                2.5f,
                3.0f,
                1.5f,
                10.0f,
                5.0f,
                0.0f,
                "North"),
            new LearningComponent(
                "comp-002",
                "space-002",
                3.0f,
                2.5f,
                2.0f,
                15.0f,
                8.0f,
                0.0f,
                "South")
        };

        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Object.LearningComponents = mockSet.Object;

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Async enumerator implementation for mocking IAsyncEnumerable in EF Core queries.
    /// </summary>
    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
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

    /// <summary>
    /// Async query provider implementation for mocking IQueryable in EF Core async operations.
    /// </summary>
    private class TestAsyncQueryProvider<TEntity> : IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
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

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression)!;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }
    }

    /// <summary>
    /// Async enumerable implementation for mocking IQueryable in EF Core async operations.
    /// </summary>
    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
