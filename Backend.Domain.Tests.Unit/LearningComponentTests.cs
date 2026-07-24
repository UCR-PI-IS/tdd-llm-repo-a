using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> domain entity.
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
        _width = 10.0f;
        _height = 5.0f;
        _depth = 2.0f;
        _x = 1.0f;
        _y = 2.0f;
        _z = 3.0f;
        _orientation = "North";
    }

    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_CreatesEntityWithExpectedProperties()
    {
        // Arrange
        // values from SetUp

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

    [Test]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        const float invalidWidth = -1.0f;

        // Act
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

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("width"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        const float invalidHeight = -1.0f;

        // Act
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

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("height"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        const float invalidDepth = -1.0f;

        // Act
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

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("depth"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException()
    {
        // Arrange
        const float invalidX = -1.0f;

        // Act
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

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("x"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException()
    {
        // Arrange
        const float invalidY = -1.0f;

        // Act
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

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("y"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException()
    {
        // Arrange
        const float invalidZ = -1.0f;

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
                invalidZ,
                _orientation));

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("z"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        const string invalidOrientation = "Northeast";

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
        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    [TestCase("North", Description = "Verify that creating a LearningComponent with orientation North succeeds")]
    [TestCase("South", Description = "Verify that creating a LearningComponent with orientation South succeeds")]
    [TestCase("East", Description = "Verify that creating a LearningComponent with orientation East succeeds")]
    [TestCase("West", Description = "Verify that creating a LearningComponent with orientation West succeeds")]
    public void Constructor_ValidOrientation_SetsOrientation(string orientation)
    {
        // Arrange
        // orientation from TestCase

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

    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
    public void Constructor_ZeroDimensionsAndCoordinates_Succeeds()
    {
        // Arrange
        const float zero = 0.0f;

        // Act
        var component = new LearningComponent(
            _componentId,
            _learningSpaceId,
            zero,
            zero,
            zero,
            zero,
            zero,
            zero,
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
