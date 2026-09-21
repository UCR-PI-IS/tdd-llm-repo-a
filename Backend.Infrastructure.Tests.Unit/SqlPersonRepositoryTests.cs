using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlPersonRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-005.
/// </summary>
[TestFixture]
public class SqlPersonRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<Person>> _mockDbSet = null!;
    private SqlPersonRepository _sut = null!;

    // Valid test data
    private const int ValidId = 1;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Person>>();

        _mockDbContext
            .Setup(c => c.Persons)
            .Returns(_mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new SqlPersonRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that a person is successfully added to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds person and calls SaveChanges")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        await _sut.AddAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(person, It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that repository returns true when email already exists in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns true when email exists")]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate)
        }.AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(ValidEmail);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository returns false when email does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository returns false when email does not exist")]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var persons = new List<Person>().AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that repository returns true when identity number already exists in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Repository returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberExists_ReturnsTrue()
    {
        // Arrange
        var persons = new List<Person>
        {
            new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate)
        }.AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(ValidIdentityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that repository returns false when identity number does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Repository returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var persons = new List<Person>().AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync("999999999");

        // Assert
        Assert.That(exists, Is.False);
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
