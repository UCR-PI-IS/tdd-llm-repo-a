using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
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

    /// <summary>
    /// Sets up common test data before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _componentId = "comp-001";
        _learningSpaceId = "space-001";
        _width = 2.5f;
        _height = 3.0f;
        _depth = 1.5f;
        _x = 10.0f;
        _y = 5.0f;
        _z = 0.0f;
        _orientation = "North";
    }

    /// <summary>
    /// Verifies that a LearningComponent entity can be created with valid parameters
    /// and that all properties are correctly initialized.
    /// </summary>
    [Test]
    [Description("Creates a LearningComponent with valid parameters and verifies all properties")]
    public void Constructor_ValidParameters_CreatesComponentWithCorrectProperties()
    {
        // Arrange
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
    /// Verifies that creating a LearningComponent with a negative dimension
    /// throws an ArgumentException with the correct parameter name.
    /// </summary>
    /// <param name="invalidWidth">Width value to test.</param>
    /// <param name="invalidHeight">Height value to test.</param>
    /// <param name="invalidDepth">Depth value to test.</param>
    /// <param name="expectedParamName">Expected parameter name in the exception.</param>
    [TestCase(-1.0f, 1.0f, 1.0f, "width", Description = "Negative width throws ArgumentException")]
    [TestCase(1.0f, -1.0f, 1.0f, "height", Description = "Negative height throws ArgumentException")]
    [TestCase(1.0f, 1.0f, -1.0f, "depth", Description = "Negative depth throws ArgumentException")]
    public void Constructor_NegativeDimension_ThrowsArgumentException(
        float invalidWidth,
        float invalidHeight,
        float invalidDepth,
        string expectedParamName)
    {
        // Arrange
        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                _componentId,
                _learningSpaceId,
                invalidWidth,
                invalidHeight,
                invalidDepth,
                _x,
                _y,
                _z,
                _orientation));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo(expectedParamName));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative coordinate
    /// throws an ArgumentException with the correct parameter name.
    /// </summary>
    /// <param name="invalidX">X coordinate value to test.</param>
    /// <param name="invalidY">Y coordinate value to test.</param>
    /// <param name="invalidZ">Z coordinate value to test.</param>
    /// <param name="expectedParamName">Expected parameter name in the exception.</param>
    [TestCase(-1.0f, 1.0f, 1.0f, "x", Description = "Negative X coordinate throws ArgumentException")]
    [TestCase(1.0f, -1.0f, 1.0f, "y", Description = "Negative Y coordinate throws ArgumentException")]
    [TestCase(1.0f, 1.0f, -1.0f, "z", Description = "Negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeCoordinate_ThrowsArgumentException(
        float invalidX,
        float invalidY,
        float invalidZ,
        string expectedParamName)
    {
        // Arrange
        // Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                _componentId,
                _learningSpaceId,
                _width,
                _height,
                _depth,
                invalidX,
                invalidY,
                invalidZ,
                _orientation));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo(expectedParamName));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with an invalid orientation
    /// throws an ArgumentException with the correct parameter name.
    /// </summary>
    [Test]
    [Description("Invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var invalidOrientation = "Invalid";

        // Act
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

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with valid orientations succeeds.
    /// </summary>
    /// <param name="orientation">Valid orientation value to test.</param>
    [TestCase("North", Description = "North orientation is valid")]
    [TestCase("South", Description = "South orientation is valid")]
    [TestCase("East", Description = "East orientation is valid")]
    [TestCase("West", Description = "West orientation is valid")]
    public void Constructor_ValidOrientation_CreatesComponent(string orientation)
    {
        // Arrange
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
    /// Verifies that creating a LearningComponent with zero values for dimensions
    /// and coordinates succeeds (boundary value testing).
    /// </summary>
    [Test]
    [Description("Zero values for dimensions and coordinates are accepted as boundary values")]
    public void Constructor_ZeroValues_CreatesComponentWithZeroProperties()
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
