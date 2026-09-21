using Microsoft.EntityFrameworkCore;
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
    [Description("Infrastructure-001: AddAsync adds person to DbSet and calls SaveChangesAsync")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var person = new Person("PER-001", "John", "Doe", "john.doe@example.com", "123456789", new DateTime(1990, 1, 1));

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
    /// Infrastructure-002: Verify that ExistsByEmailAsync returns true when email already exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByEmailAsync returns true when email exists")]
    public async Task ExistsByEmailAsync_ExistingEmail_ReturnsTrue()
    {
        // Arrange
        var email = "john.doe@example.com";
        var personList = new List<Person>
        {
            new Person("PER-001", "John", "Doe", email, "123456789", new DateTime(1990, 1, 1))
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(personList.Provider);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personList.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personList.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personList.GetEnumerator());

        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);
        _sut = new SqlPersonRepository(_mockDbContext.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByEmailAsync returns false when email does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByEmailAsync returns false when email does not exist")]
    public async Task ExistsByEmailAsync_NonExistingEmail_ReturnsFalse()
    {
        // Arrange
        var email = "notfound@example.com";
        var personList = new List<Person>().AsQueryable();

        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(personList.Provider);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personList.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personList.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personList.GetEnumerator());

        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);
        _sut = new SqlPersonRepository(_mockDbContext.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(email);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that ExistsByIdentityNumberAsync returns true when identity number already exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: ExistsByIdentityNumberAsync returns true when identity number exists")]
    public async Task ExistsByIdentityNumberAsync_ExistingIdentityNumber_ReturnsTrue()
    {
        // Arrange
        var identityNumber = "123456789";
        var personList = new List<Person>
        {
            new Person("PER-001", "John", "Doe", "john.doe@example.com", identityNumber, new DateTime(1990, 1, 1))
        }.AsQueryable();

        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(personList.Provider);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personList.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personList.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personList.GetEnumerator());

        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);
        _sut = new SqlPersonRepository(_mockDbContext.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that ExistsByIdentityNumberAsync returns false when identity number does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: ExistsByIdentityNumberAsync returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_NonExistingIdentityNumber_ReturnsFalse()
    {
        // Arrange
        var identityNumber = "999999999";
        var personList = new List<Person>().AsQueryable();

        var mockDbSet = new Mock<DbSet<Person>>();
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(personList.Provider);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(personList.Expression);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(personList.ElementType);
        mockDbSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(personList.GetEnumerator());

        _mockDbContext.Setup(c => c.Persons).Returns(mockDbSet.Object);
        _sut = new SqlPersonRepository(_mockDbContext.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(identityNumber);

        // Assert
        Assert.That(exists, Is.False);
    }
}
