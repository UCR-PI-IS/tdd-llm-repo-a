using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;
using MockQueryable.Moq;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the SqlLearningComponentRepository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;
    private string _learningSpaceId = null!;

    /// <summary>
    /// Sets up test fixtures before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<UCRDatabaseContext>();
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
        _learningSpaceId = "LS-001";
    }

    /// <summary>
    /// Verifies repository returns list of components for a valid learning space ID from database.
    /// </summary>
    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidLearningSpaceId_ReturnsListOfComponents()
    {
        // Arrange
        var expectedComponents = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", _learningSpaceId, 2.5f, 1.8f, 0.5f, 1.0f, 0.0f, 2.0f, "North"),
            new LearningComponent("COMP-002", _learningSpaceId, 1.5f, 1.5f, 0.4f, 3.0f, 0.0f, 2.0f, "South")
        };

        var mockDbSet = expectedComponents.AsQueryable().BuildMockDbSet();

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(c => c.LearningSpaceId == _learningSpaceId), Is.True);
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space has no components.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_LearningSpaceHasNoComponents_ReturnsEmptyList()
    {
        // Arrange
        var emptyList = new List<LearningComponent>();
        var mockDbSet = emptyList.AsQueryable().BuildMockDbSet();

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
    }

    /// <summary>
    /// Verifies repository returns empty list when learning space ID does not exist in database.
    /// </summary>
    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentLearningSpaceId_ReturnsEmptyList()
    {
        // Arrange
        string nonExistentLearningSpaceId = "LS-999";
        var existingComponents = new List<LearningComponent>
        {
            new LearningComponent("COMP-001", "LS-OTHER", 2.5f, 1.8f, 0.5f, 1.0f, 0.0f, 2.0f, "North")
        };

        var mockDbSet = existingComponents.AsQueryable().BuildMockDbSet();

        _mockDbContext
            .Setup(ctx => ctx.LearningComponents)
            .Returns(mockDbSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(0));
        Assert.That(result, Is.Empty);
    }
}
