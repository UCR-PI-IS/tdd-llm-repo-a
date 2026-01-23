using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the LearningComponent entity.
/// User Story: CPD-LC-001-001 - List components in a learning space
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    [Test(Description = "Verify LearningComponent can be constructed with valid ComponentId and LearningSpaceId")]
    public void Constructor_WithValidParameters_ShouldSetComponentIdAndLearningSpaceId()
    {
        // Arrange & Act
        var component = new LearningComponent(
            componentId: 1,
            learningSpaceId: "LS-001",
            width: 2.0f,
            height: 1.5f,
            depth: 0.5f,
            x: 1.0f,
            y: 0.0f,
            z: 3.0f,
            orientation: "North"
        );

        // Assert
        Assert.That(component.ComponentId, Is.EqualTo(1));
        Assert.That(component.LearningSpaceId, Is.EqualTo("LS-001"));
    }

    [Test(Description = "Verify LearningComponent can be constructed with valid dimension properties")]
    public void Constructor_WithValidParameters_ShouldSetDimensions()
    {
        // Arrange & Act
        var component = new LearningComponent(
            componentId: 1,
            learningSpaceId: "LS-001",
            width: 2.0f,
            height: 1.5f,
            depth: 0.5f,
            x: 1.0f,
            y: 0.0f,
            z: 3.0f,
            orientation: "North"
        );

        // Assert
        Assert.That(component.Width, Is.EqualTo(2.0f));
        Assert.That(component.Height, Is.EqualTo(1.5f));
        Assert.That(component.Depth, Is.EqualTo(0.5f));
    }

    [Test(Description = "Verify LearningComponent can be constructed with valid position coordinates")]
    public void Constructor_WithValidParameters_ShouldSetPositionCoordinates()
    {
        // Arrange & Act
        var component = new LearningComponent(
            componentId: 1,
            learningSpaceId: "LS-001",
            width: 2.0f,
            height: 1.5f,
            depth: 0.5f,
            x: 1.0f,
            y: 0.0f,
            z: 3.0f,
            orientation: "North"
        );

        // Assert
        Assert.That(component.X, Is.EqualTo(1.0f));
        Assert.That(component.Y, Is.EqualTo(0.0f));
        Assert.That(component.Z, Is.EqualTo(3.0f));
    }

    [Test(Description = "Verify LearningComponent can be constructed with valid orientation")]
    public void Constructor_WithValidParameters_ShouldSetOrientation()
    {
        // Arrange & Act
        var component = new LearningComponent(
            componentId: 1,
            learningSpaceId: "LS-001",
            width: 2.0f,
            height: 1.5f,
            depth: 0.5f,
            x: 1.0f,
            y: 0.0f,
            z: 3.0f,
            orientation: "North"
        );

        // Assert
        Assert.That(component.Orientation, Is.EqualTo("North"));
    }

    [TestCase("North", TestName = "North orientation")]
    [TestCase("South", TestName = "South orientation")]
    [TestCase("East", TestName = "East orientation")]
    [TestCase("West", TestName = "West orientation")]
    [Test(Description = "Verify LearningComponent can be constructed with different valid orientations")]
    public void Constructor_WithDifferentOrientations_ShouldSetOrientationCorrectly(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            componentId: 1,
            learningSpaceId: "LS-001",
            width: 2.0f,
            height: 1.5f,
            depth: 0.5f,
            x: 1.0f,
            y: 0.0f,
            z: 3.0f,
            orientation: orientation
        );

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }
}
