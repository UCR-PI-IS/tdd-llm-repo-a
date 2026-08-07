using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the ILearningComponentRepository interface contract.
/// </summary>
[TestFixture]
public class ILearningComponentRepositoryTests
{
    private Mock<ILearningComponentRepository> _mockRepository = null!;

    /// <summary>
    /// Sets up the test context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<ILearningComponentRepository>();
    }

    /// <summary>
    /// Verifies that GetComponentsByLearningSpaceIdAsync method exists and can be called.
    /// </summary>
    [Test]
    [Description("Verify that GetComponentsByLearningSpaceIdAsync method exists and can be called")]
    public void GetComponentsByLearningSpaceIdAsync_MethodExists()
    {
        // Arrange
        String learningSpaceId = "LS-001";
        var expectedComponents = new List<LearningComponent>();
        
        _mockRepository
            .Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(expectedComponents);

        // Act
        var result = _mockRepository.Object.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        _mockRepository.Verify(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }
}
