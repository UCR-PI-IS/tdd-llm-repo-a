using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the LearningComponent entity.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private string _componentId = null!;
    private string _learningSpaceId = null!;
    private float _width;
    private float _height;
    private float _depth;
    private float _x;
    private float _y;
    private float _z;
    private string _orientation = null!;

    [SetUp]
    public void SetUp()
    {
        _componentId = "comp-001";
        _learningSpaceId = "space-001";
        _width = 2.5f;
        _height = 3.0f;
        _depth = 2.0f;
        _x = 1.0f;
        _y = 0.5f;
        _z = 2.0f;
        _orientation = "North";
    }

    /// <summary>
    /// Test that a LearningComponent can be created with valid parameters.
    /// </summary>
    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_CreatesComponent()
    {
        // Arrange - values initialized in SetUp

        // Act
        var component = new LearningComponent(
            _componentId, 
            _learningSpaceId, 
            _width, 
            _height, 
            _depth, 
            _x, 
            _y, 
            _z, 
            _orientation);

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
    /// Test that creating a LearningComponent with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var invalidWidth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                invalidWidth, 
                _height, 
                _depth, 
                _x, 
                _y, 
                _z, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var invalidHeight = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                invalidHeight, 
                _depth, 
                _x, 
                _y, 
                _z, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with negative depth throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        var invalidDepth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                _height, 
                invalidDepth, 
                _x, 
                _y, 
                _z, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("depth"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with negative X coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException()
    {
        // Arrange
        var invalidX = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                _height, 
                _depth, 
                invalidX, 
                _y, 
                _z, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("x"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with negative Y coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException()
    {
        // Arrange
        var invalidY = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                _height, 
                _depth, 
                _x, 
                invalidY, 
                _z, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("y"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with negative Z coordinate throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException()
    {
        // Arrange
        var invalidZ = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                _height, 
                _depth, 
                _x, 
                _y, 
                invalidZ, 
                _orientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("z"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var invalidOrientation = "Invalid";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => 
            new LearningComponent(
                _componentId, 
                _learningSpaceId, 
                _width, 
                _height, 
                _depth, 
                _x, 
                _y, 
                _z, 
                invalidOrientation));
        
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Test that creating a LearningComponent with valid orientations succeeds.
    /// </summary>
    [TestCase("North")]
    [TestCase("South")]
    [TestCase("East")]
    [TestCase("West")]
    [Description("Verify that creating a LearningComponent with valid orientations (North, South, East, West) succeeds")]
    public void Constructor_ValidOrientations_CreatesComponent(string orientation)
    {
        // Arrange - values initialized in SetUp

        // Act
        var component = new LearningComponent(
            _componentId, 
            _learningSpaceId, 
            _width, 
            _height, 
            _depth, 
            _x, 
            _y, 
            _z, 
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Test that creating a LearningComponent with zero values for dimensions and coordinates succeeds.
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
    public void Constructor_ZeroValues_CreatesComponent()
    {
        // Arrange
        var zeroWidth = 0f;
        var zeroHeight = 0f;
        var zeroDepth = 0f;
        var zeroX = 0f;
        var zeroY = 0f;
        var zeroZ = 0f;

        // Act
        var component = new LearningComponent(
            _componentId, 
            _learningSpaceId, 
            zeroWidth, 
            zeroHeight, 
            zeroDepth, 
            zeroX, 
            zeroY, 
            zeroZ, 
            _orientation);

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
