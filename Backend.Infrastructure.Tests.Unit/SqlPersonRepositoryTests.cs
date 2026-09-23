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

    // Valid test data
    private const int ValidId = 1;
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@email.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 5, 15);

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Person>>();

        // Set up async interfaces BEFORE accessing .Object
        SetupAsyncMockDbSet(new List<Person>());

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
    /// The repository should call AddAsync on the DbSet and SaveChangesAsync on the context.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds person to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        await _sut.AddAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(
                It.Is<Person>(p =>
                    p.Id == ValidId &&
                    p.Email == ValidEmail &&
                    p.IdentityNumber == ValidIdentityNumber),
                It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByEmailAsync returns true when the email
    /// already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByEmailAsync returns true when email exists")]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var existingPerson = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        SetupAsyncMockDbSet(new List<Person> { existingPerson });

        // Act
        var exists = await _sut.ExistsByEmailAsync(ValidEmail);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByEmailAsync returns false when the email
    /// does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByEmailAsync returns false when email does not exist")]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var existingPerson = new Person(ValidId, ValidFirstName, ValidLastName, "other@email.com", ValidIdentityNumber, ValidBirthDate);
        SetupAsyncMockDbSet(new List<Person> { existingPerson });

        // Act
        var exists = await _sut.ExistsByEmailAsync(ValidEmail);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that ExistsByIdentityNumberAsync returns true when the
    /// identity number already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: ExistsByIdentityNumberAsync returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberExists_ReturnsTrue()
    {
        // Arrange
        var existingPerson = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        SetupAsyncMockDbSet(new List<Person> { existingPerson });

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(ValidIdentityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that ExistsByIdentityNumberAsync returns false when the
    /// identity number does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: ExistsByIdentityNumberAsync returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var existingPerson = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, "999999999", ValidBirthDate);
        SetupAsyncMockDbSet(new List<Person> { existingPerson });

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(ValidIdentityNumber);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Sets up the mock DbSet to support async LINQ queries (AnyAsync, etc.)
    /// using an in-memory data source.
    /// </summary>
    private void SetupAsyncMockDbSet(List<Person> data)
    {
        var queryable = data.AsQueryable();

        _mockDbSet.As<IAsyncEnumerable<Person>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Person>(data.GetEnumerator()));

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Person>(queryable.Provider));

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
    }

    #region Async Queryable Mock Helpers

    /// <summary>
    /// Async query provider that wraps a synchronous query provider,
    /// enabling EF Core async extension methods (e.g., AnyAsync) to work with in-memory data.
    /// </summary>
    private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider, IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object? Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var resultType = typeof(TResult);

            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var actualType = resultType.GetGenericArguments()[0];
                var syncResult = typeof(IQueryProvider)
                    .GetMethod(nameof(IQueryProvider.Execute), 1, new[] { typeof(Expression) })!
                    .MakeGenericMethod(actualType)
                    .Invoke(_inner, new object[] { expression });

                return (TResult)typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(actualType)
                    .Invoke(null, new[] { syncResult })!;
            }

            return _inner.Execute<TResult>(expression);
        }
    }

    /// <summary>
    /// Async-enumerable queryable that bridges synchronous LINQ with EF Core async methods.
    /// </summary>
    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider
            => new TestAsyncQueryProvider<T>(this);
    }

    /// <summary>
    /// Async enumerator wrapper that delegates to a synchronous enumerator.
    /// </summary>
    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
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
            return new ValueTask();
        }

        public ValueTask<bool> MoveNextAsync()
            => new ValueTask<bool>(_inner.MoveNext());
    }

    #endregion
}
