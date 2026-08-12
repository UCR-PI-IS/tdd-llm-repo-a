using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.TestHelpers;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories;

/// <summary>
/// Unit tests for <see cref="SqlLearningComponentRepository"/>.
/// Uses Moq to mock the <see cref="UCRDatabaseContext"/> and its <see cref="DbSet{LearningComponent}"/>.
/// </summary>
[TestFixture]
public class SqlLearningComponentRepositoryTests
{
    private Mock<UCRDatabaseContext> _mockContext = null!;
    private SqlLearningComponentRepository _repository = null!;

    private static LearningComponent CreateComponent(string componentId, string learningSpaceId)
    {
        return new LearningComponent(componentId, learningSpaceId, 2.0f, 1.5f, 0.5f, 1.0f, 2.0f, 0.0f, "North");
    }

    [SetUp]
    public void SetUp()
    {
        var dbContextOptions = new DbContextOptions<UCRDatabaseContext>();
        _mockContext = new Mock<UCRDatabaseContext>(dbContextOptions);
    }

    private void SetupMockDbSet(List<LearningComponent> data)
    {
        var mockDbSet = MockDbSetHelper.CreateMockDbSet(data);
        _mockContext.Setup(c => c.LearningComponents).Returns(mockDbSet.Object);
        _repository = new SqlLearningComponentRepository(_mockContext.Object);
    }

    [Test(Description = "Verify repository returns list of components for a valid learning space ID from database")]
    public async Task GetComponentsByLearningSpaceIdAsync_ValidId_ReturnsComponents()
    {
        // Arrange
        var learningSpaceId = "LS001";
        var components = new List<LearningComponent>
        {
            CreateComponent("C001", learningSpaceId),
            CreateComponent("C002", learningSpaceId)
        };
        SetupMockDbSet(components);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(c => c.LearningSpaceId == learningSpaceId), Is.True);
        });
    }

    [Test(Description = "Verify repository returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        var learningSpaceId = "LS002";
        var componentsForOtherSpace = new List<LearningComponent>
        {
            CreateComponent("C001", "LS001"),
            CreateComponent("C002", "LS001")
        };
        SetupMockDbSet(componentsForOtherSpace);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }

    [Test(Description = "Verify repository returns empty list when learning space ID does not exist in database")]
    public async Task GetComponentsByLearningSpaceIdAsync_NonExistentId_ReturnsEmptyList()
    {
        // Arrange
        var nonExistentLearningSpaceId = "NONEXISTENT";
        var existingComponents = new List<LearningComponent>
        {
            CreateComponent("C001", "LS001")
        };
        SetupMockDbSet(existingComponents);

        // Act
        var result = await _repository.GetComponentsByLearningSpaceIdAsync(nonExistentLearningSpaceId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        });
    }
}
