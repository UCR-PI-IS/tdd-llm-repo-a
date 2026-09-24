using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlUniversityRepository"/>.
/// Covers intents Infrastructure-001 through Infrastructure-003.
/// </summary>
[TestFixture]
public class SqlUniversityRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<University>> _mockDbSet = null!;
    private SqlUniversityRepository _sut = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<University>>();
        _sut = new SqlUniversityRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-001: Verify that a valid university entity is persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds university to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidUniversity_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var university = new University("UCR", "Costa Rica");

        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(_mockDbSet.Object);
        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _sut.AddAsync(university);

        // Assert
        Assert.Multiple(() =>
        {
            _mockDbSet.Verify(d => d.AddAsync(university, It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true when university with given name exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: ExistsByNameAsync returns true when name exists")]
    public async Task ExistsByNameAsync_NameExists_ReturnsTrue()
    {
        // Arrange
        var universities = new List<University> { new University("UCR", "Costa Rica") }.AsQueryable();

        var mockDbSet = new Mock<DbSet<University>>();
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Provider).Returns(universities.Provider);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Expression).Returns(universities.Expression);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.ElementType).Returns(universities.ElementType);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.GetEnumerator()).Returns(universities.GetEnumerator());

        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.ExistsByNameAsync("UCR");

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false when university with given name does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: ExistsByNameAsync returns false when name does not exist")]
    public async Task ExistsByNameAsync_NameDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var universities = new List<University>().AsQueryable();

        var mockDbSet = new Mock<DbSet<University>>();
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Provider).Returns(universities.Provider);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Expression).Returns(universities.Expression);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.ElementType).Returns(universities.ElementType);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.GetEnumerator()).Returns(universities.GetEnumerator());

        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _sut.ExistsByNameAsync("UCR");

        // Assert
        Assert.That(result, Is.False);
    }
}
