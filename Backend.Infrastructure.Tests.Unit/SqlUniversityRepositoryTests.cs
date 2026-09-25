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
    /// Infrastructure-001: Verify that the repository adds a university entity
    /// to the DbSet and calls SaveChanges to persist it to the database.
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
    /// Infrastructure-002 and Infrastructure-003: Verify that ExistsByNameAsync returns
    /// the correct boolean result depending on whether a university with the given name exists.
    /// </summary>
    [TestCase("UCR", true,
        Description = "Infrastructure-002: Returns true when university with given name exists")]
    [TestCase("UCR", false,
        Description = "Infrastructure-003: Returns false when university with given name does not exist")]
    public async Task ExistsByNameAsync_WithOrWithoutMatchingUniversity_ReturnsExpectedResult(
        string searchName, bool shouldExist)
    {
        // Arrange
        if (shouldExist)
        {
            var university = new University("UCR", "Costa Rica");
            await _dbContext.Universities.AddAsync(university);
            await _dbContext.SaveChangesAsync();
        }

        // Act
        var result = await _sut.ExistsByNameAsync(searchName);

        // Assert
        Assert.That(result, Is.EqualTo(shouldExist));
    }
}
