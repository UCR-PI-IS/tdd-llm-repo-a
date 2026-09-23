using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
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
    private static readonly DateTime ValidBirthDate = new(1990, 5, 15);

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
    /// Infrastructure-002: Verify that repository returns true when email already exists in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByEmailAsync returns true when email exists")]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var email = ValidEmail;
        var person = new Person(ValidId, ValidFirstName, ValidLastName, email, ValidIdentityNumber, ValidBirthDate);
        var persons = new List<Person> { person }.AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository returns false when email does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByEmailAsync returns false when email does not exist")]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var persons = new List<Person>().AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that repository returns true when identity number already exists in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: ExistsByIdentityNumberAsync returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberExists_ReturnsTrue()
    {
        // Arrange
        var identityNumber = ValidIdentityNumber;
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, identityNumber, ValidBirthDate);
        var persons = new List<Person> { person }.AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that repository returns false when identity number does not exist in database.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: ExistsByIdentityNumberAsync returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var identityNumber = "999999999";
        var persons = new List<Person>().AsQueryable();

        var mockDbSet = CreateMockDbSet(persons);
        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.False);
    }

    private static Mock<DbSet<Person>> CreateMockDbSet(IQueryable<Person> data)
    {
        var mockDbSet = new Mock<DbSet<Person>>();

        mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Provider)
            .Returns(new AsyncQueryProvider<Person>(data.Provider));

        mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Expression)
            .Returns(data.Expression);

        mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.ElementType)
            .Returns(data.ElementType);

        mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => data.GetEnumerator());

        return mockDbSet;
    }

    /// <summary>
    /// Async query provider that enables LINQ operations on mocked DbSets.
    /// </summary>
    private class AsyncQueryProvider<TEntity> : IQueryProvider
    {
        private readonly IQueryProvider _inner;

        public AsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new AsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncEnumerable<TElement>(expression);
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
    /// Async enumerable wrapper that enables LINQ operations on in-memory data.
    /// </summary>
    private class AsyncEnumerable<T> : EnumerableQuery<T>, IQueryable<T>
    {
        public AsyncEnumerable(Expression expression) : base(expression)
        {
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new AsyncQueryProvider<T>(this); }
        }
    }
}
