using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<ApplicationDbContext> _mockContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<ApplicationDbContext>(MockBehavior.Strict);
        _repository = new SqlLearningComponentRepository(_mockContext.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockContext.VerifyAll();
    }

    /// <summary>
    /// Test that repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithComponents_ReturnsList()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                learningSpaceId,
                10.0f,
                5.0f,
                8.0f,
                1.0f,
                2.0f,
                3.0f,
                "North"),
            new LearningComponent(
                "comp-002",
                learningSpaceId,
                12.0f,
                6.0f,
                9.0f,
                4.0f,
                5.0f,
                6.0f,
                "South")
        };

        var mockSet = new Mock<DbSet<LearningComponent>>();
        mockSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(components.GetEnumerator()));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(components.AsQueryable().Provider));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(components.AsQueryable().Expression);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(components.AsQueryable().ElementType);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(components.GetEnumerator());

        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockSet.Object);

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
    /// Test that repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "space-001";
        var components = new List<LearningComponent>();

        var mockSet = new Mock<DbSet<LearningComponent>>();
        mockSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(components.GetEnumerator()));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(components.AsQueryable().Provider));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(components.AsQueryable().Expression);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(components.AsQueryable().ElementType);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(components.GetEnumerator());

        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockSet.Object);

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
    /// Test that repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentSpace_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "non-existent-space";
        var components = new List<LearningComponent>
        {
            new LearningComponent(
                "comp-001",
                "space-001",
                10.0f,
                5.0f,
                8.0f,
                1.0f,
                2.0f,
                3.0f,
                "North")
        };

        var mockSet = new Mock<DbSet<LearningComponent>>();
        mockSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(components.GetEnumerator()));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(components.AsQueryable().Provider));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(components.AsQueryable().Expression);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(components.AsQueryable().ElementType);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(components.GetEnumerator());

        _mockContext
            .Setup(c => c.LearningComponents)
            .Returns(mockSet.Object);

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
}

/// <summary>
/// Helper class for async query provider.
/// </summary>
internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
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

    public object Execute(Expression expression)
    {
        return _inner.Execute(expression)!;
    }

    public TResult Execute<TResult>(Expression expression)
    {
        return _inner.Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
    {
        return new TestAsyncEnumerable<TResult>(expression);
    }

    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        return Execute<TResult>(expression);
    }
}

/// <summary>
/// Helper class for async enumerable.
/// </summary>
internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable)
        : base(enumerable)
    { }

    public TestAsyncEnumerable(Expression expression)
        : base(expression)
    { }

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
/// Helper class for async enumerator.
/// </summary>
internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner)
    {
        _inner = inner;
    }

    public T Current
    {
        get { return _inner.Current; }
    }

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(_inner.MoveNext());
    }
}
