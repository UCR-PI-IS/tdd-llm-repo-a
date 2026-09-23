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
    private static readonly Person ValidPerson = new(
        1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Person>>();

        _mockDbContext
            .Setup(c => c.Persons)
            .Returns(() => _mockDbSet.Object);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _sut = new SqlPersonRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that AddAsync adds a person to the DbSet
    /// and calls SaveChangesAsync to persist it to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds person to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var person = new Person(1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));

        // Act
        await _sut.AddAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(
                It.Is<Person>(p => p.Email == "john.doe@example.com" && p.IdentityNumber == "123456789"),
                It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByEmailAsync returns true
    /// when a person with the specified email already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByEmailAsync returns true when email exists")]
    public async Task ExistsByEmailAsync_EmailExists_ReturnsTrue()
    {
        // Arrange
        var existingPerson = new Person(1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));
        var persons = new List<Person> { existingPerson };
        SetupMockDbSetWithAsyncQueryable(persons);

        // Act
        var exists = await _sut.ExistsByEmailAsync("john.doe@example.com");

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByEmailAsync returns false
    /// when no person with the specified email exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByEmailAsync returns false when email does not exist")]
    public async Task ExistsByEmailAsync_EmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var existingPerson = new Person(1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));
        var persons = new List<Person> { existingPerson };
        SetupMockDbSetWithAsyncQueryable(persons);

        // Act
        var exists = await _sut.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that ExistsByIdentityNumberAsync returns true
    /// when a person with the specified identity number already exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: ExistsByIdentityNumberAsync returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberExists_ReturnsTrue()
    {
        // Arrange
        var existingPerson = new Person(1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));
        var persons = new List<Person> { existingPerson };
        SetupMockDbSetWithAsyncQueryable(persons);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync("123456789");

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that ExistsByIdentityNumberAsync returns false
    /// when no person with the specified identity number exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: ExistsByIdentityNumberAsync returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_IdentityNumberDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var existingPerson = new Person(1, "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 5, 15));
        var persons = new List<Person> { existingPerson };
        SetupMockDbSetWithAsyncQueryable(persons);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync("999999999");

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Configures the mock DbSet to support async LINQ queries against the provided in-memory data.
    /// </summary>
    private void SetupMockDbSetWithAsyncQueryable(List<Person> data)
    {
        var asyncQueryable = data.AsAsyncQueryable();

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Provider)
            .Returns(asyncQueryable.Provider);

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.Expression)
            .Returns(asyncQueryable.Expression);

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.ElementType)
            .Returns(asyncQueryable.ElementType);

        _mockDbSet.As<IQueryable<Person>>()
            .Setup(m => m.GetEnumerator())
            .Returns(asyncQueryable.GetEnumerator());

        _mockDbSet.As<IAsyncEnumerable<Person>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<Person>(data.GetEnumerator()));
    }
}
