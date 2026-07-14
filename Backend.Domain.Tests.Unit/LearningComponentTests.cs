using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the LearningComponent entity.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private string _componentId;
    private string _learningSpaceId;
    private float _width;
    private float _height;
    private float _depth;
    private float _x;
    private float _y;
    private float _z;
    private string _orientation;

    [SetUp]
    public void SetUp()
    {
        _componentId = "COMP-001";
        _learningSpaceId = "SPACE-001";
        _width = 2.0f;
        _height = 1.5f;
        _depth = 1.0f;
        _x = 0.5f;
        _y = 0.0f;
        _z = 1.0f;
        _orientation = "North";
    }

    /// <summary>
    /// Verifies that a LearningComponent entity can be created with valid parameters.
    /// </summary>
    [Test]
    [Description("Creates LearningComponent with valid parameters and verifies all properties")]
    public void Constructor_WithValidParameters_CreatesComponent()
    {
        // Arrange
        // Act
        var component = new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, _orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.EqualTo(_componentId));
            Assert.That(component.LearningSpaceId, Is.EqualTo(_learningSpaceId));
            Assert.That(component.Width, Is.EqualTo(_width));
            Assert.That(component.Height, Is.EqualTo(_height));
            Assert.That(component.Depth, Is.EqualTo(_depth));
            Assert.That(component.X, Is.EqualTo(_x));
            Assert.That(component.Y, Is.EqualTo(_y));
            Assert.That(component.Z, Is.EqualTo(_z));
            Assert.That(component.Orientation, Is.EqualTo(_orientation));
        });
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when width is negative")]
    public void Constructor_WithNegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        float invalidWidth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, invalidWidth, _height, _depth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when height is negative")]
    public void Constructor_WithNegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        float invalidHeight = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, invalidHeight, _depth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative depth throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when depth is negative")]
    public void Constructor_WithNegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        float invalidDepth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, invalidDepth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("depth"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative X coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when X coordinate is negative")]
    public void Constructor_WithNegativeX_ThrowsArgumentException()
    {
        // Arrange
        float invalidX = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, invalidX, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("x"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative Y coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when Y coordinate is negative")]
    public void Constructor_WithNegativeY_ThrowsArgumentException()
    {
        // Arrange
        float invalidY = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, invalidY, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("y"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with negative Z coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when Z coordinate is negative")]
    public void Constructor_WithNegativeZ_ThrowsArgumentException()
    {
        // Arrange
        float invalidZ = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, invalidZ, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo("z"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Throws ArgumentException when orientation is invalid")]
    public void Constructor_WithInvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        string invalidOrientation = "Invalid";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, invalidOrientation));
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with valid orientations succeeds.
    /// </summary>
    [TestCase("North")]
    [TestCase("South")]
    [TestCase("East")]
    [TestCase("West")]
    [Description("Creates LearningComponent with valid orientations")]
    public void Constructor_WithValidOrientations_CreatesComponent(string orientation)
    {
        // Arrange
        // Act
        var component = new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with zero values for dimensions and coordinates succeeds.
    /// </summary>
    [Test]
    [Description("Creates LearningComponent with zero values (boundary test)")]
    public void Constructor_WithZeroValues_CreatesComponent()
    {
        // Arrange
        float zeroWidth = 0f;
        float zeroHeight = 0f;
        float zeroDepth = 0f;
        float zeroX = 0f;
        float zeroY = 0f;
        float zeroZ = 0f;

        // Act
        var component = new LearningComponent(_componentId, _learningSpaceId, zeroWidth, zeroHeight, zeroDepth, zeroX, zeroY, zeroZ, _orientation);

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
