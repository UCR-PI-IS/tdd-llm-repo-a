using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for the PersonRepository.
/// Tests cover database operations for person management.
/// </summary>
[TestFixture]
public class PersonRepositoryTests
{
    private DbContextOptions<UCRDatabaseContext> _options;

    [SetUp]
    public void SetUp()
    {
        _options = new DbContextOptionsBuilder<UCRDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Test(Description = "AddAsync should successfully save person to database")]
    public async Task AddAsync_WithValidPerson_ShouldSaveToDatabase()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var repository = new PersonRepository(context);

        var person = new Person(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            "123456789",
            new DateTime(1990, 1, 1),
            "555-1234"
        );

        // Act
        var result = await repository.AddAsync(person);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(context.Persons.Count(), Is.EqualTo(1));

        var savedPerson = await context.Persons.FirstOrDefaultAsync();
        Assert.That(savedPerson, Is.Not.Null);
        Assert.That(savedPerson.Email, Is.EqualTo("john.doe@example.com"));
        Assert.That(savedPerson.FirstName, Is.EqualTo("John"));
        Assert.That(savedPerson.LastName, Is.EqualTo("Doe"));
        Assert.That(savedPerson.IdentityNumber, Is.EqualTo("123456789"));
    }

    [Test(Description = "GetByEmailAsync should return person when email exists")]
    public async Task GetByEmailAsync_WhenEmailExists_ShouldReturnPerson()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var person = new Person(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            "123456789",
            new DateTime(1990, 1, 1),
            "555-1234"
        );
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var repository = new PersonRepository(context);

        // Act
        var result = await repository.GetByEmailAsync("john.doe@example.com");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Email, Is.EqualTo("john.doe@example.com"));
        Assert.That(result.FirstName, Is.EqualTo("John"));
        Assert.That(result.LastName, Is.EqualTo("Doe"));
    }

    [Test(Description = "GetByEmailAsync should return null when email does not exist")]
    public async Task GetByEmailAsync_WhenEmailDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var repository = new PersonRepository(context);

        // Act
        var result = await repository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test(Description = "GetByIdentityNumberAsync should return person when identity number exists")]
    public async Task GetByIdentityNumberAsync_WhenIdentityNumberExists_ShouldReturnPerson()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var person = new Person(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com",
            "123456789",
            new DateTime(1990, 1, 1),
            "555-1234"
        );
        context.Persons.Add(person);
        await context.SaveChangesAsync();

        var repository = new PersonRepository(context);

        // Act
        var result = await repository.GetByIdentityNumberAsync("123456789");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IdentityNumber, Is.EqualTo("123456789"));
        Assert.That(result.FirstName, Is.EqualTo("John"));
        Assert.That(result.Email, Is.EqualTo("john.doe@example.com"));
    }

    [Test(Description = "GetByIdentityNumberAsync should return null when identity number does not exist")]
    public async Task GetByIdentityNumberAsync_WhenIdentityNumberDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        using var context = new UCRDatabaseContext(_options);
        var repository = new PersonRepository(context);

        // Act
        var result = await repository.GetByIdentityNumberAsync("999999999");

        // Assert
        Assert.That(result, Is.Null);
    }
}
