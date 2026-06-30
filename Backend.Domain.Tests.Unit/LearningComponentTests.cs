using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_CreatesEntity()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 1.0f;
        var y = 2.0f;
        var z = 0.5f;
        var orientation = Orientation.North;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.EqualTo(componentId));
            Assert.That(component.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(component.Width, Is.EqualTo(width));
            Assert.That(component.Height, Is.EqualTo(height));
            Assert.That(component.Depth, Is.EqualTo(depth));
            Assert.That(component.X, Is.EqualTo(x));
            Assert.That(component.Y, Is.EqualTo(y));
            Assert.That(component.Z, Is.EqualTo(z));
            Assert.That(component.Orientation, Is.EqualTo(orientation));
        });
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var invalidWidth = -1.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, invalidWidth, height, depth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var invalidHeight = -1.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, invalidHeight, depth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var invalidDepth = -1.0f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, invalidDepth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("depth"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var invalidX = -1.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, invalidX, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("x"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var invalidY = -1.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, invalidY, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("y"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var y = 0.0f;
        var invalidZ = -1.0f;
        var orientation = Orientation.North;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, invalidZ, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("z"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var invalidOrientation = (Orientation)99;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, invalidOrientation));
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    [TestCase(Orientation.North)]
    [TestCase(Orientation.South)]
    [TestCase(Orientation.East)]
    [TestCase(Orientation.West)]
    [Description("Verify that creating a LearningComponent with valid orientations succeeds")]
    public void Constructor_ValidOrientations_Succeeds(Orientation orientation)
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 2.0f;
        var height = 3.0f;
        var depth = 1.5f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds")]
    public void Constructor_ZeroValues_Succeeds()
    {
        // Arrange
        var componentId = "comp-001";
        var learningSpaceId = "space-001";
        var width = 0.0f;
        var height = 0.0f;
        var depth = 0.0f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = Orientation.North;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.Width, Is.EqualTo(0.0f));
            Assert.That(component.Height, Is.EqualTo(0.0f));
            Assert.That(component.Depth, Is.EqualTo(0.0f));
            Assert.That(component.X, Is.EqualTo(0.0f));
            Assert.That(component.Y, Is.EqualTo(0.0f));
            Assert.That(component.Z, Is.EqualTo(0.0f));
        });
    }
}
