using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private const string ValidComponentId = "COMP-001";
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 10.0f;
    private const float ValidHeight = 5.0f;
    private const float ValidDepth = 8.0f;
    private const float ValidX = 2.0f;
    private const float ValidY = 3.0f;
    private const float ValidZ = 1.0f;
    private const string ValidOrientation = "North";

    #region Positive Tests

    /// <summary>
    /// Verify that a LearningComponent entity can be created with valid parameters
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_ShouldCreateEntityWithCorrectProperties()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

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
    /// Verify that creating a LearningComponent with valid orientations
    /// (North, South, East, West) succeeds for each orientation value.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with valid orientations (North, South, East, West) succeeds")]
    public void Constructor_ValidOrientations_ShouldCreateEntityWithCorrectOrientation(
        [Values("North", "South", "East", "West")] string orientation)
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with zero values for dimensions
    /// and coordinates succeeds (boundary test).
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
    public void Constructor_ZeroValuesForDimensionsAndCoordinates_ShouldCreateEntityWithZeroValues()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = 0f;
        var height = 0f;
        var depth = 0f;
        var x = 0f;
        var y = 0f;
        var z = 0f;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

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

    #endregion

    #region Negative Tests

    /// <summary>
    /// Verify that creating a LearningComponent with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var invalidWidth = -1.0f;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, invalidWidth, height, depth, x, y, z, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var invalidHeight = -1.0f;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, invalidHeight, depth, x, y, z, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with negative depth throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var invalidDepth = -1.0f;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, invalidDepth, x, y, z, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("depth"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with negative X coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeXCoordinate_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var invalidX = -1.0f;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, invalidX, y, z, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("x"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeYCoordinate_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var invalidY = -1.0f;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, invalidY, z, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("y"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZCoordinate_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var invalidZ = -1.0f;
        var orientation = ValidOrientation;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, invalidZ, orientation));
        Assert.That(ex!.ParamName, Is.EqualTo("z"));
    }

    /// <summary>
    /// Verify that creating a LearningComponent with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ShouldThrowArgumentException()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var invalidOrientation = "InvalidOrientation";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, invalidOrientation));
        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    #endregion
}