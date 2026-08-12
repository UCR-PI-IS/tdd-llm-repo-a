using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/>.
/// Uses Moq to mock the <see cref="UCRDatabaseContext"/> and <see cref="DbSet{T}"/>.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    private const string ValidLearningSpaceId = "LS-001";
    private const string NonExistentLearningSpaceId = "LS-999";

    [SetUp]
    public void SetUp()
    {
        var dbContextOptions = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(dbContextOptions);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockDbContext.Reset();
    }

    /// <summary>
    /// Infrastructure-001: Verifies the repository returns a list of components
    /// for a valid learning space ID from the database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsComponentList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", learningSpaceId, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, "North"),
            new LearningComponent("COMP-002", learningSpaceId, 2.0f, 2.0f, 2.0f, 1.0f, 1.0f, 1.0f, "South")
        };

        var mockDbSet = CreateMockDbSet(components);
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verifies the repository returns an empty list
    /// when the learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = ValidLearningSpaceId;
        var emptyComponents = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(emptyComponents);
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Infrastructure-003: Verifies the repository returns an empty list
    /// when the learning space ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = NonExistentLearningSpaceId;
        var componentsForOtherSpace = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", "LS-002", 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, "North")
        };

        var mockDbSet = CreateMockDbSet(componentsForOtherSpace);
        _mockDbContext
            .Setup(c => c.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Creates a mock <see cref="DbSet{T}"/> backed by the given in-memory data,
    /// with support for synchronous LINQ queries and async enumeration (ToListAsync).
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

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

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

        return mockSet;
    }
}

#region Async LINQ helpers for mocked DbSet

/// <summary>
/// An <see cref="IAsyncEnumerable{T}"/> wrapper around <see cref="EnumerableQuery{T}"/>
/// that enables async LINQ operations (e.g. ToListAsync) on a mocked DbSet.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }
}

/// <summary>
/// Query provider that creates <see cref="TestAsyncEnumerable{T}"/> instances,
/// enabling Where/Select/etc. to produce async-enumerable results.
/// </summary>
internal class TestAsyncQueryProvider<T> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner)
    {
        _inner = inner;
    }

    public IQueryable CreateQuery(Expression expression)
        => new TestAsyncEnumerable<T>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object? Execute(Expression expression)
        => _inner.Execute(expression);

    public TResult Execute<TResult>(Expression expression)
        => _inner.Execute<TResult>(expression);
}

/// <summary>
/// Async enumerator wrapper around a synchronous <see cref="IEnumerator{T}"/>.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return default;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }
}

#endregion
