using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
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
        _componentId = "comp-001";
        _learningSpaceId = "space-001";
        _width = 10.0f;
        _height = 5.0f;
        _depth = 8.0f;
        _x = 1.0f;
        _y = 2.0f;
        _z = 3.0f;
        _orientation = "North";
    }

    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_CreatesEntity()
    {
        // Arrange
        // Act
        var component = new LearningComponent(
            _componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, _orientation);

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
        float invalidWidth = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, invalidWidth, _height, _depth, _x, _y, _z, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("width"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        float invalidHeight = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, invalidHeight, _depth, _x, _y, _z, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("height"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        float invalidDepth = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, _height, invalidDepth, _x, _y, _z, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("depth"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException()
    {
        // Arrange
        float invalidX = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, invalidX, _y, _z, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("x"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException()
    {
        // Arrange
        float invalidY = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, invalidY, _z, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("y"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException()
    {
        // Arrange
        float invalidZ = -1.0f;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, invalidZ, _orientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("z"));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        string invalidOrientation = "Invalid";

        // Act & Assert
        Assert.That(
            () => new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, invalidOrientation),
            Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("orientation"));
    }

    [TestCase("North")]
    [TestCase("South")]
    [TestCase("East")]
    [TestCase("West")]
    [Description("Verify that creating a LearningComponent with valid orientations succeeds")]
    public void Constructor_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange
        // Act
        var component = new LearningComponent(
            _componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds")]
    public void Constructor_ZeroValuesForDimensionsAndCoordinates_Succeeds()
    {
        // Arrange
        float zero = 0.0f;

        // Act
        var component = new LearningComponent(
            _componentId, _learningSpaceId, zero, zero, zero, zero, zero, zero, _orientation);

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
