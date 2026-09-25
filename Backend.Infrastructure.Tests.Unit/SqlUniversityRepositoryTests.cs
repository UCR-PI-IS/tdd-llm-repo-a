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
    /// The repository calls AddAsync on the DbSet and SaveChangesAsync on the context.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds university to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidUniversity_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var university = new University("UCR", "Costa Rica");

        // Act
        await _sut.AddAsync(university);

        // Assert
        var added = await _dbContext.Universities.FirstOrDefaultAsync(u => u.Name == "UCR");
        Assert.That(added, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(added.Name, Is.EqualTo("UCR"));
            Assert.That(added.Country, Is.EqualTo("Costa Rica"));
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true
    /// when a university with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when university exists")]
    public async Task ExistsByNameAsync_UniversityExists_ReturnsTrue()
    {
        // Arrange
        var university = new University("UCR", "Costa Rica");
        await _dbContext.Universities.AddAsync(university);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsByNameAsync("UCR");

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false
    /// when no university with the given name exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when university does not exist")]
    public async Task ExistsByNameAsync_UniversityDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = await _sut.ExistsByNameAsync("UCR");

        // Assert
        Assert.That(result, Is.False);
    }
}
