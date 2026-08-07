using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for the ILearningComponentService interface contract.
/// </summary>
[TestFixture]
public class ILearningComponentServiceTests
{
    private Mock<ILearningComponentService> _mockService = null!;

    /// <summary>
    /// Sets up the test context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<ILearningComponentService>();
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
        
        _mockService
            .Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(expectedComponents);

        // Act
        var result = _mockService.Object.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Not.Null);
        _mockService.Verify(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId), Times.Once);
    }
}
