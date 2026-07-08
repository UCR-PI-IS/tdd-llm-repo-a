using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    /// <summary>
    /// Sets up mocks and SUT before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>(new DbContextOptions<UCRDatabaseContext>());
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _mockDbContext.Setup(db => db.LearningComponents).Returns(_mockDbSet.Object);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Creates a test LearningComponent with specified parameters.
    /// </summary>
    private LearningComponent CreateTestComponent(String componentId, String learningSpaceId)
    {
        return new LearningComponent(
            componentId,
            learningSpaceId,
            10.0f,  // width
            5.0f,   // height
            8.0f,   // depth
            1.0f,   // x
            2.0f,   // y
            3.0f,   // z
            "North");
    }

    /// <summary>
    /// Helper method to create a mock DbSet from a list of entities.
    /// </summary>
    private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> entities) where T : class
    {
        var queryable = entities.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();
        
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        
        mockDbSet.As<IAsyncEnumerable<T>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));
        
        return mockDbSet;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        
        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }
        
        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }
        
        public T Current => _inner.Current;
        
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }

    private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
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

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute(expression);
            return (TResult)result;
        }
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression) : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithValidId_ReturnsListOfComponents()
    {
        // Arrange
        String learningSpaceId = "space-001";
        var expectedComponents = new List<LearningComponent>
        {
            CreateTestComponent("component-001", learningSpaceId),
            CreateTestComponent("component-002", learningSpaceId)
        };

        var mockDbSet = CreateMockDbSet(expectedComponents);
        _mockDbContext.Setup(db => db.LearningComponents).Returns(mockDbSet.Object);

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
    /// Verifies repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        String learningSpaceId = "space-002";
        var emptyList = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(emptyList);
        _mockDbContext.Setup(db => db.LearningComponents).Returns(mockDbSet.Object);

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
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentId_ReturnsEmptyList()
    {
        // Arrange
        String nonExistentLearningSpaceId = "space-nonexistent";
        var emptyList = new List<LearningComponent>();

        var mockDbSet = CreateMockDbSet(emptyList);
        _mockDbContext.Setup(db => db.LearningComponents).Returns(mockDbSet.Object);

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
