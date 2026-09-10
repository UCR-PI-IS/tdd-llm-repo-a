using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository.AddAsync"/>.
/// Covers intents Infrastructure-004 and Infrastructure-005 for story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryAddTests
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
        _mockDbContext.Setup(c => c.LearningComponents).Returns(_mockDbSet.Object);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    /// <summary>
    /// Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges.
    /// </summary>
    [Test]
    [Description("Infrastructure-004: Verify that adding a component with a unique ID succeeds and calls SaveChanges")]
    public async Task AddAsync_WithUniqueId_SucceedsAndCallsSaveChanges()
    {
        // Arrange
        var component = new LearningComponent("COMP-12345", "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");

        // Act
        await _repository.AddAsync(component);

        // Assert
        _mockDbSet.Verify(x => x.AddAsync(component, It.IsAny<CancellationToken>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Infrastructure-005: Verify that adding a component with a duplicate ID throws a DbUpdateException due to unique constraint violation.
    /// </summary>
    [Test]
    [Description("Infrastructure-005: Verify that adding a component with a duplicate ID throws a DbUpdateException")]
    public async Task AddAsync_WithDuplicateId_ThrowsDbUpdateException()
    {
        // Arrange
        var component = new LearningComponent("COMP-12345", "LS-001", 1.5f, 1.0f, 0.5f, 10.0f, 5.0f, 0.0f, "North");
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException("Duplicate key"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<DbUpdateException>(() => _repository.AddAsync(component));
        Assert.That(ex.Message, Does.Contain("Duplicate key"));
    }
}
