using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/>.
/// Uses Moq to mock <see cref="UCRDatabaseContext"/> and its <see cref="DbSet{T}"/>
/// with full async-queryable support so that EF Core's ToListAsync works correctly.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    private const string ValidLearningSpaceId = "IF-0103";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>().Options;
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningComponent>>();

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(() => _mockDbSet.Object);

        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockDbContext = null!;
        _mockDbSet = null!;
        _repository = null!;
    }

    // ── Helper: configure mock DbSet as async-queryable ─────────────────

    /// <summary>
    /// Configures the mock <see cref="DbSet{T}"/> to behave as an async-queryable
    /// collection backed by the supplied in-memory list.
    /// </summary>
    private static void SetupMockDbSetAsync(
        Mock<DbSet<LearningComponent>> mockDbSet,
        List<LearningComponent> data)
    {
        var queryable = data.AsQueryable();

        // IQueryable support (for Where, Select, etc.)
        mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(queryable.Provider));

        mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        // IAsyncEnumerable support (for ToListAsync)
        mockDbSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(() => new TestAsyncEnumerator<LearningComponent>(data.GetEnumerator()));
    }

    // ────────────────────────────────────────────────────────────────────
    // Infrastructure-001  –  Returns components for valid learning space ID
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Infrastructure-001: Verify repository returns list of components for a valid learning space ID from database.")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsComponentList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", ValidLearningSpaceId, 2f, 1.5f, 0.5f, 1f, 2f, 0f, "North"),
            new LearningComponent("COMP-002", ValidLearningSpaceId, 3f, 2f, 1f, 3f, 4f, 0f, "South")
        };
        SetupMockDbSetAsync(_mockDbSet, components);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == ValidLearningSpaceId), Is.True);
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Infrastructure-002  –  Returns empty list when no components exist
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Infrastructure-002: Verify repository returns empty list when learning space has no components.")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        SetupMockDbSetAsync(_mockDbSet, new List<LearningComponent>());

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(ValidLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    // ────────────────────────────────────────────────────────────────────
    // Infrastructure-003  –  Returns empty list for non-existent ID
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Infrastructure-003: Verify repository returns empty list when learning space ID does not exist in database.")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange — database has components for a different learning space
        var components = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", ValidLearningSpaceId, 2f, 1.5f, 0.5f, 1f, 2f, 0f, "North")
        };
        SetupMockDbSetAsync(_mockDbSet, components);

        // Act — query for a learning space that does not exist
        var result = await _repository.GetComponentsByLearningSpaceIdAsync("NON-EXISTENT-ID");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }
}

// ════════════════════════════════════════════════════════════════════════
//  Async-queryable helpers for mocking EF Core DbSet with Moq
// ════════════════════════════════════════════════════════════════════════

/// <summary>
/// An <see cref="IQueryProvider"/> that creates <see cref="TestAsyncEnumerable{T}"/>
/// instances so that LINQ operators (Where, Select, …) produce async-enumerable results.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IQueryProvider
{
    private readonly IQueryProvider _inner;

    internal TestAsyncQueryProvider(IQueryProvider inner)
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

    public object? Execute(Expression expression)
    {
        return _inner.Execute(expression);
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }
}

/// <summary>
/// An <see cref="IQueryable{T}"/> that also implements <see cref="IAsyncEnumerable{T}"/>,
/// enabling EF Core's <c>ToListAsync</c> to iterate over in-memory data.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    internal TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    {
    }

    internal TestAsyncEnumerable(Expression expression)
        : base(expression)
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    }

    IQueryProvider IQueryable.Provider
    {
        get { return new TestAsyncQueryProvider<T>(this); }
    }
}

/// <summary>
/// Wraps a synchronous <see cref="IEnumerator{T}"/> as an <see cref="IAsyncEnumerator{T}"/>.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    internal TestAsyncEnumerator(IEnumerator<T> inner)
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
