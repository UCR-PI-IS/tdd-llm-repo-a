using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlUniversityRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
/// Uses EF Core In-Memory database to avoid mocking unsupported extension methods.
/// </summary>
[TestFixture]
public class SqlUniversityRepositoryTests
{
    private UCRDatabaseContext _dbContext = null!;
    private SqlUniversityRepository _sut = null!;

    // Valid test data
    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new UCRDatabaseContext(options);
        _sut = new SqlUniversityRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    /// <summary>
    /// Infrastructure-001: Verify that a valid university entity is persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds university to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidUniversity_PersistsToDatabase()
    {
        // Arrange
        var university = new University(ValidName, ValidCountry);

        // Act
        await _sut.AddAsync(university);

        // Assert
        var added = await _dbContext.Universities.FirstOrDefaultAsync(u => u.Name == ValidName);
        Assert.Multiple(() =>
        {
            Assert.That(added, Is.Not.Null);
            Assert.That(added!.Name, Is.EqualTo(ValidName));
            Assert.That(added.Country, Is.EqualTo(ValidCountry));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true when a university
    /// with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns true when university name exists")]
    public async Task ExistsByNameAsync_NameExists_ReturnsTrue()
    {
        // Arrange
        var university = new University(ValidName, ValidCountry);
        await _dbContext.Universities.AddAsync(university);
        await _dbContext.SaveChangesAsync();

        // Act
        var exists = await _sut.ExistsByNameAsync(ValidName);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false when no university
    /// with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository returns false when university name does not exist")]
    public async Task ExistsByNameAsync_NameDoesNotExist_ReturnsFalse()
    {
        // Act
        var exists = await _sut.ExistsByNameAsync("Nonexistent University");

        // Assert
        Assert.That(exists, Is.False);
    }
}
