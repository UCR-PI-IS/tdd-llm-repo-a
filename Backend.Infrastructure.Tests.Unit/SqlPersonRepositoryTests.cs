using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Person>>();

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new SqlPersonRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that the repository adds a person to the DbSet
    /// and calls SaveChangesAsync to persist it to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds person to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndSavesChanges()
    {
        // Arrange
        var person = new Person(1, "John", "Doe", "john@example.com", "ID-001", new DateTime(2000, 1, 1));
        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);

        // Act
        await _sut.AddAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(
                It.Is<Person>(p =>
                    p.Email == "john@example.com" &&
                    p.IdentityNumber == "ID-001"),
                It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByEmailAsync returns true
    /// when a person with the given email already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByEmailAsync returns true when email exists")]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var email = "john@example.com";
        var persons = new List<Person>
        {
            new(1, "John", "Doe", email, "ID-001", new DateTime(2000, 1, 1))
        };
        SetupMockDbSetAsQueryable(_mockDbSet, persons);
        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByEmailAsync returns false
    /// when no person with the given email exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByEmailAsync returns false when email does not exist")]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var persons = new List<Person>
        {
            new(1, "John", "Doe", "john@example.com", "ID-001", new DateTime(2000, 1, 1))
        };
        SetupMockDbSetAsQueryable(_mockDbSet, persons);
        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that ExistsByIdentityNumberAsync returns true
    /// when a person with the given identity number already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: ExistsByIdentityNumberAsync returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberExists_ReturnsTrue()
    {
        // Arrange
        var identityNumber = "ID-001";
        var persons = new List<Person>
        {
            new(1, "John", "Doe", "john@example.com", identityNumber, new DateTime(2000, 1, 1))
        };
        SetupMockDbSetAsQueryable(_mockDbSet, persons);
        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that ExistsByIdentityNumberAsync returns false
    /// when no person with the given identity number exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: ExistsByIdentityNumberAsync returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var identityNumber = "ID-999";
        var persons = new List<Person>
        {
            new(1, "John", "Doe", "john@example.com", "ID-001", new DateTime(2000, 1, 1))
        };
        SetupMockDbSetAsQueryable(_mockDbSet, persons);
        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.False);
    }

    #region Helper Methods

    /// <summary>
    /// Sets up a mock DbSet as an IQueryable with async query provider support
    /// so that EF Core async extension methods (e.g., AnyAsync) work correctly.
    /// </summary>
    private static void SetupMockDbSetAsQueryable<T>(Mock<DbSet<T>> mockDbSet, List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var asyncProvider = new TestAsyncQueryProvider<T>(queryable.Provider);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider).Returns(asyncProvider);
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
    }

    /// <summary>
    /// A test async query provider that wraps a standard LINQ query provider
    /// and adds support for EF Core async extension methods.
    /// </summary>
    private class TestAsyncQueryProvider<TEntity> : IQueryProvider, IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new AsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncEnumerable<TElement>(expression);

        public object? Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GenericTypeArguments.Length > 0
                ? typeof(TResult).GenericTypeArguments[0]
                : typeof(TResult);

            var result = typeof(IQueryProvider)
                .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                .MakeGenericMethod(expectedResultType)
                .Invoke(_inner, new[] { expression });

            return (TResult)typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { result })!;
        }
    }

    private class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public AsyncEnumerable(Expression expression) : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
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

    #endregion
}
