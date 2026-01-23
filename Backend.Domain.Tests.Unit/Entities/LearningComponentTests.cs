using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the LearningComponent entity.
/// Tests verify proper construction and property initialization.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    [Test]
    [Description("Verify LearningComponent correctly initializes ComponentId and LearningSpaceId")]
    public void Constructor_ValidParameters_SetsComponentIdAndLearningSpaceId()
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

    [Test]
    [Description("Verify LearningComponent correctly initializes dimension properties")]
    public void Constructor_ValidParameters_SetsDimensions()
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

    [Test]
    [Description("Verify LearningComponent correctly initializes position coordinates")]
    public void Constructor_ValidParameters_SetsPosition()
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

    [Test]
    [Description("Verify LearningComponent correctly initializes orientation")]
    public void Constructor_ValidParameters_SetsOrientation()
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

    [TestCase("North", TestName = "Constructor with North orientation")]
    [TestCase("South", TestName = "Constructor with South orientation")]
    [TestCase("East", TestName = "Constructor with East orientation")]
    [TestCase("West", TestName = "Constructor with West orientation")]
    [Description("Verify LearningComponent can be constructed with different valid orientations")]
    public void Constructor_DifferentOrientations_SetsOrientationCorrectly(string orientation)
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
