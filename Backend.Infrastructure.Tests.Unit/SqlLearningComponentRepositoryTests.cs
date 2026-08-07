using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
    }

    #region GetComponentsByLearningSpaceIdAsync Tests

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verifies repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithExistingComponents_ReturnsListOfComponents()
    {
        // Arrange
        var learningSpaceId = "ls-001";
        var components = new List<LearningComponent>
        {
            new("comp-001", learningSpaceId, 2.5f, 3.0f, 2.0f, 10.0f, 5.0f, 0.0f, "North"),
            new("comp-002", learningSpaceId, 1.5f, 2.0f, 1.5f, 15.0f, 8.0f, 0.0f, "South")
        };

        _mockDbContext
            .Setup(db => db.LearningComponents)
            .ReturnsDbSet(components);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verifies repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "ls-empty";
        var emptyComponents = new List<LearningComponent>();

        _mockDbContext
            .Setup(db => db.LearningComponents)
            .ReturnsDbSet(emptyComponents);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verifies repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_WithNonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "ls-nonexistent";
        var existingComponents = new List<LearningComponent>
        {
            new("comp-001", "ls-001", 2.5f, 3.0f, 2.0f, 10.0f, 5.0f, 0.0f, "North"),
            new("comp-002", "ls-002", 1.5f, 2.0f, 1.5f, 15.0f, 8.0f, 0.0f, "South")
        };

        _mockDbContext
            .Setup(db => db.LearningComponents)
            .ReturnsDbSet(existingComponents);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    #endregion
}
