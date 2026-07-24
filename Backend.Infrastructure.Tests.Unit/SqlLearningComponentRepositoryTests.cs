using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="SqlLearningComponentRepository"/> infrastructure repository.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockDbContext = null!;
    private SqlLearningComponentRepository _repository = null!;
    private string _learningSpaceId = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<UCRDatabaseContext>().Options;
        _mockDbContext = new Mock<UCRDatabaseContext>(options);
        _repository = new SqlLearningComponentRepository(_mockDbContext.Object);
        _learningSpaceId = "space-001";
    }

    private static Mock<DbSet<LearningComponent>> CreateMockDbSet(List<LearningComponent> data)
    {
        var queryable = data.AsQueryable();
        var mockSet = new Mock<DbSet<LearningComponent>>();

        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<LearningComponent>(queryable.Provider));
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.Expression)
            .Returns(queryable.Expression);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.ElementType)
            .Returns(queryable.ElementType);
        mockSet.As<IQueryable<LearningComponent>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => queryable.GetEnumerator());
        mockSet.As<IAsyncEnumerable<LearningComponent>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<LearningComponent>(queryable.GetEnumerator()));

        return mockSet;
    }

    [Test]
    [Description("Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents_ReturnsList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("comp-001", _learningSpaceId, 10f, 5f, 2f, 0f, 0f, 0f, "North"),
            new LearningComponent("comp-002", _learningSpaceId, 8f, 4f, 1f, 1f, 1f, 0f, "South")
        };
        var mockSet = CreateMockDbSet(components);
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == _learningSpaceId), Is.True);
        });
    }

    [Test]
    [Description("Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var mockSet = CreateMockDbSet(new List<LearningComponent>());
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(_learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));
            Assert.That(result, Is.Empty);
        });
    }

    [Test]
    [Description("Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "space-999";
        var otherComponents = new List<LearningComponent>
        {
            new LearningComponent("comp-001", _learningSpaceId, 10f, 5f, 2f, 0f, 0f, 0f, "North")
        };
        var mockSet = CreateMockDbSet(otherComponents);
        _mockDbContext.Setup(c => c.LearningComponents).Returns(mockSet.Object);

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
}
