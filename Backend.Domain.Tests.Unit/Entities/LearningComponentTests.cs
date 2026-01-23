using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for LearningComponent entity
/// User Story: CPD-LC-001-001 - List components in a learning space
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    [Test]
    [TestCase(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North", Description = "Verify LearningComponent construction with valid parameters")]
    [TestCase(100, "LS-999", 5.0f, 3.0f, 1.0f, 10.0f, 5.0f, 15.0f, "South", Description = "Verify LearningComponent construction with different valid parameters")]
    public void Constructor_WithValidParameters_ShouldSetComponentIdAndLearningSpaceId(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.ComponentId, Is.EqualTo(componentId));
        Assert.That(component.LearningSpaceId, Is.EqualTo(learningSpaceId));
    }

    [Test]
    [TestCase(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North", Description = "Verify LearningComponent dimensions are set correctly")]
    [TestCase(2, "LS-002", 3.5f, 2.5f, 0.8f, 2.0f, 1.0f, 4.0f, "East", Description = "Verify LearningComponent dimensions with different values")]
    public void Constructor_WithValidParameters_ShouldSetDimensions(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Width, Is.EqualTo(width));
        Assert.That(component.Height, Is.EqualTo(height));
        Assert.That(component.Depth, Is.EqualTo(depth));
    }

    [Test]
    [TestCase(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North", Description = "Verify LearningComponent position coordinates are set correctly")]
    [TestCase(2, "LS-002", 3.0f, 2.0f, 0.3f, 5.5f, 2.5f, 7.5f, "West", Description = "Verify LearningComponent position with different coordinates")]
    public void Constructor_WithValidParameters_ShouldSetPosition(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.X, Is.EqualTo(x));
        Assert.That(component.Y, Is.EqualTo(y));
        Assert.That(component.Z, Is.EqualTo(z));
    }

    [Test]
    [TestCase(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North", Description = "Verify LearningComponent orientation is set correctly")]
    [TestCase(2, "LS-002", 3.0f, 2.0f, 0.3f, 2.0f, 1.0f, 4.0f, "South", Description = "Verify LearningComponent orientation with South value")]
    public void Constructor_WithValidParameters_ShouldSetOrientation(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    [Test]
    [TestCase(1, "LS-001", 2.0f, 1.5f, 0.5f, 1.0f, 0.0f, 3.0f, "North", 2, "LS-001", 3.0f, 2.0f, 0.3f, 2.0f, 1.0f, 4.0f, "South", Description = "Verify different orientations can be set on different components")]
    [TestCase(3, "LS-003", 1.5f, 1.0f, 0.2f, 0.5f, 0.0f, 1.5f, "East", 4, "LS-003", 2.5f, 2.0f, 0.4f, 3.0f, 1.5f, 5.0f, "West", Description = "Verify East and West orientations")]
    public void Constructor_WithDifferentValidOrientations_ShouldSetOrientationCorrectly(
        int componentId1, string learningSpaceId1, float width1, float height1, float depth1, float x1, float y1, float z1, string orientation1,
        int componentId2, string learningSpaceId2, float width2, float height2, float depth2, float x2, float y2, float z2, string orientation2)
    {
        // Act
        var component1 = new LearningComponent(componentId1, learningSpaceId1, width1, height1, depth1, x1, y1, z1, orientation1);
        var component2 = new LearningComponent(componentId2, learningSpaceId2, width2, height2, depth2, x2, y2, z2, orientation2);

        // Assert
        Assert.That(component1.Orientation, Is.EqualTo(orientation1));
        Assert.That(component2.Orientation, Is.EqualTo(orientation2));
    }
}
