using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlPersonRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-005.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlPersonRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
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
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _sut = new SqlPersonRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Infrastructure-001: Verify that a person is successfully added to the database.
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
        var added = await _dbContext.Persons.FirstOrDefaultAsync(p => p.Id == ValidId);
        Assert.That(added, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(added.FirstName, Is.EqualTo(ValidFirstName));
            Assert.That(added.LastName, Is.EqualTo(ValidLastName));
            Assert.That(added.Email, Is.EqualTo(ValidEmail));
            Assert.That(added.IdentityNumber, Is.EqualTo(ValidIdentityNumber));
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
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();

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
        var person = new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
        await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();

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
        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync("999999999");

        // Assert
        Assert.That(exists, Is.False);
    }
}
