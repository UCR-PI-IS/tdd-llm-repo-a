using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    private static readonly Guid ValidId = Guid.NewGuid();
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 15);

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<Person>>();

        _mockDbContext.Setup(c => c.Persons).Returns(_mockDbSet.Object);
        _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _sut = new SqlPersonRepository(_mockDbContext.Object);
    }

    private static Person CreateValidPerson()
    {
        return new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
    }

    private static Mock<DbSet<Person>> SetupMockDbSetWithData(List<Person> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<Person>>();
        mockSet.As<IQueryable<Person>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<Person>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<Person>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<Person>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        return mockSet;
    }

    /// <summary>
    /// Infrastructure-001: Verify that a person is successfully added to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Verify that a person is successfully added to the database")]
    public async Task AddAsync_ValidPerson_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var person = CreateValidPerson();

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
    /// Infrastructure-002: Verify that repository returns true when email already exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify that repository returns true when email already exists")]
    public async Task ExistsByEmailAsync_ExistingEmail_ReturnsTrue()
    {
        // Arrange
        var person = CreateValidPerson();
        var data = new List<Person> { person };
        var mockSet = SetupMockDbSetWithData(data);
        _mockDbContext.Setup(c => c.Persons).Returns(mockSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(ValidEmail);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that repository returns false when email does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify that repository returns false when email does not exist")]
    public async Task ExistsByEmailAsync_NonExistingEmail_ReturnsFalse()
    {
        // Arrange
        var data = new List<Person>();
        var mockSet = SetupMockDbSetWithData(data);
        _mockDbContext.Setup(c => c.Persons).Returns(mockSet.Object);

        // Act
        var exists = await _sut.ExistsByEmailAsync(ValidEmail);

        // Assert
        Assert.That(exists, Is.False);
    }

    /// <summary>
    /// Infrastructure-004: Verify that repository returns true when identity number already exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify that repository returns true when identity number already exists")]
    public async Task ExistsByIdentityNumberAsync_ExistingIdentityNumber_ReturnsTrue()
    {
        // Arrange
        var person = CreateValidPerson();
        var data = new List<Person> { person };
        var mockSet = SetupMockDbSetWithData(data);
        _mockDbContext.Setup(c => c.Persons).Returns(mockSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(ValidIdentityNumber);

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-005: Verify that repository returns false when identity number does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify that repository returns false when identity number does not exist")]
    public async Task ExistsByIdentityNumberAsync_NonExistingIdentityNumber_ReturnsFalse()
    {
        // Arrange
        var data = new List<Person>();
        var mockSet = SetupMockDbSetWithData(data);
        _mockDbContext.Setup(c => c.Persons).Returns(mockSet.Object);

        // Act
        var exists = await _sut.ExistsByIdentityNumberAsync(ValidIdentityNumber);

        // Assert
        Assert.That(exists, Is.False);
    }
}
