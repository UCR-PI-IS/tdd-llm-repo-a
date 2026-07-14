namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

using System.Collections.Generic;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Unit tests for the LearningComponent entity.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private const string ValidComponentId = "component-001";
    private const string ValidLearningSpaceId = "space-001";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 3.0f;
    private const float ValidDepth = 2.0f;
    private const float ValidX = 1.0f;
    private const float ValidY = 2.0f;
    private const float ValidZ = 0.5f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Tests that all properties are correctly set when creating a LearningComponent with valid parameters.
    /// </summary>
    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_WithValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;
        string orientation = ValidOrientation;

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

    /// <summary>
    /// Tests that creating a LearningComponent with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_WithNegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float invalidWidth = -1.0f;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, invalidWidth, height, depth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_WithNegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float invalidHeight = -1.0f;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, invalidHeight, depth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with negative depth throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_WithNegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float invalidDepth = -1.0f;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, invalidDepth, x, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("depth"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with negative X coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_WithNegativeX_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float invalidX = -1.0f;
        float y = ValidY;
        float z = ValidZ;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, invalidX, y, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("x"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with negative Y coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_WithNegativeY_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float invalidY = -1.0f;
        float z = ValidZ;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, invalidY, z, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("y"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with negative Z coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_WithNegativeZ_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float invalidZ = -1.0f;
        string orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, invalidZ, orientation));
        Assert.That(ex.ParamName, Is.EqualTo("z"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_WithInvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;
        string invalidOrientation = "Invalid";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, invalidOrientation));
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with valid orientations succeeds.
    /// </summary>
    [TestCase("North", Description = "Verify that LearningComponent can be created with North orientation")]
    [TestCase("South", Description = "Verify that LearningComponent can be created with South orientation")]
    [TestCase("East", Description = "Verify that LearningComponent can be created with East orientation")]
    [TestCase("West", Description = "Verify that LearningComponent can be created with West orientation")]
    [Description("Verify that creating a LearningComponent with valid orientations (North, South, East, West) succeeds")]
    public void Constructor_WithValidOrientations_Succeeds(string orientation)
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = ValidWidth;
        float height = ValidHeight;
        float depth = ValidDepth;
        float x = ValidX;
        float y = ValidY;
        float z = ValidZ;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Tests that creating a LearningComponent with zero values for dimensions and coordinates succeeds.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
    public void Constructor_WithZeroValues_SetsPropertiesCorrectly()
    {
        // Arrange
        string componentId = ValidComponentId;
        string learningSpaceId = ValidLearningSpaceId;
        float width = 0f;
        float height = 0f;
        float depth = 0f;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        string orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.Width, Is.EqualTo(0f));
            Assert.That(component.Height, Is.EqualTo(0f));
            Assert.That(component.Depth, Is.EqualTo(0f));
            Assert.That(component.X, Is.EqualTo(0f));
            Assert.That(component.Y, Is.EqualTo(0f));
            Assert.That(component.Z, Is.EqualTo(0f));
        });
    }
}
