using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository.ExistsAsync"/>.
/// Covers intents Infrastructure-002 and Infrastructure-003 for story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryExistsTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private Mock<DbSet<LearningComponent>> _mockDbSet = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptions<UCRDatabaseContext>();
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _mockDbSet = new Mock<DbSet<LearningComponent>>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-002: Verify that checking existence returns true when a component with the given ID exists in the database")]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var data = new List<LearningComponent>
        {
            new LearningComponent("COMP-12345", "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North")
        }.AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<LearningComponent>(data.Provider));
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(data.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockDbSet.As<IAsyncEnumerable<LearningComponent>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<LearningComponent>(data.GetEnumerator()));
        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        // Act
        var exists = await _repository.ExistsAsync("COMP-12345");

        // Assert
        Assert.That(exists, Is.True);
    }

    /// <summary>
    /// Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist in the database.
    /// </summary>
    [Test]
    [Description("Infrastructure-003: Verify that checking existence returns false when a component with the given ID does not exist in the database")]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        var data = new List<LearningComponent>().AsQueryable();

        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<LearningComponent>(data.Provider));
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.Expression).Returns(data.Expression);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.ElementType).Returns(data.ElementType);
        _mockDbSet.As<IQueryable<LearningComponent>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        _mockDbSet.As<IAsyncEnumerable<LearningComponent>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestAsyncEnumerator<LearningComponent>(data.GetEnumerator()));
        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);

        // Act
        var exists = await _repository.ExistsAsync("COMP-NONEXISTENT");

        // Assert
        Assert.That(exists, Is.False);
    }
}
