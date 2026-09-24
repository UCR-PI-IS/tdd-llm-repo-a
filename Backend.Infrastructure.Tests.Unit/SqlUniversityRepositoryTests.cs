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
    private SqlUniversityRepository _sut = null!;

    private const string ValidName = "UCR";
    private const string ValidCountry = "Costa Rica";

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);

        _mockDbContext
            .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
    }

    /// <summary>
    /// Infrastructure-001: Verify that a valid university entity is persisted to the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-001: Repository adds university to DbSet and calls SaveChanges")]
    public async Task AddAsync_ValidUniversity_AddsToDbSetAndCallsSaveChanges()
    {
        // Arrange
        var mockDbSet = new Mock<DbSet<University>>();
        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(mockDbSet.Object);
        
        _sut = new SqlUniversityRepository(_mockDbContext.Object);
        var university = new University(ValidName, ValidCountry);

        // Act
        await _sut.AddAsync(university);

        // Assert
        Assert.Multiple(() =>
        {
            mockDbSet.Verify(d => d.AddAsync(
                It.Is<University>(u => u.Name == ValidName && u.Country == ValidCountry),
                It.IsAny<CancellationToken>()),
                Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        });
    }

    /// <summary>
    /// Infrastructure-002: Verify that ExistsByNameAsync returns true when university with given name exists.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Repository returns true when university with given name exists")]
    public async Task ExistsByNameAsync_NameExists_ReturnsTrue()
    {
        // Arrange
        var universities = new List<University> { new University(ValidName, ValidCountry) }.AsQueryable();
        var mockDbSet = CreateMockDbSet(universities);
        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(mockDbSet.Object);
        
        _sut = new SqlUniversityRepository(_mockDbContext.Object);

        // Act
        var result = await _sut.ExistsByNameAsync(ValidName);

        // Assert
        Assert.That(result, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that ExistsByNameAsync returns false when university with given name does not exist.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Repository returns false when university with given name does not exist")]
    public async Task ExistsByNameAsync_NameDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var universities = new List<University>().AsQueryable();
        var mockDbSet = CreateMockDbSet(universities);
        _mockDbContext
            .Setup(c => c.Universities)
            .Returns(mockDbSet.Object);
        
        _sut = new SqlUniversityRepository(_mockDbContext.Object);

        // Act
        var result = await _sut.ExistsByNameAsync(ValidName);

        // Assert
        Assert.That(result, Is.False);
    }

    /// <summary>
    /// Creates a mock DbSet that properly implements IQueryable for LINQ operations.
    /// </summary>
    private static Mock<DbSet<University>> CreateMockDbSet(IQueryable<University> data)
    {
        var mockDbSet = new Mock<DbSet<University>>();
        
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Provider).Returns(data.Provider);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.Expression).Returns(data.Expression);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockDbSet.As<IQueryable<University>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
        
        return mockDbSet;
    }
}
