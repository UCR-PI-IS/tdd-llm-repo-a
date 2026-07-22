using System;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

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
        _learningSpaceId = "ls-001";
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
    public void Constructor_ValidParameters_CreatesEntityWithCorrectProperties()
    {
        // Arrange
        // values from SetUp

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

    [TestCase(-1.0f, "width")]
    [TestCase(-0.1f, "width")]
    [Description("Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException(float invalidWidth, string expectedParam)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, invalidWidth, _height, _depth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase(-1.0f, "height")]
    [TestCase(-0.1f, "height")]
    [Description("Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException(float invalidHeight, string expectedParam)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, invalidHeight, _depth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase(-1.0f, "depth")]
    [TestCase(-0.1f, "depth")]
    [Description("Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException(float invalidDepth, string expectedParam)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, invalidDepth, _x, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase(-1.0f, "x")]
    [TestCase(-0.1f, "x")]
    [Description("Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException(float invalidX, string expectedParam)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, invalidX, _y, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase(-1.0f, "y")]
    [TestCase(-0.1f, "y")]
    [Description("Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException(float invalidY, string expectedParam)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, invalidY, _z, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase(-1.0f, "z")]
    [TestCase(-0.1f, "z")]
    [Description("Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException(float invalidZ, string expectedParam)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, invalidZ, _orientation));
        Assert.That(ex.ParamName, Is.EqualTo(expectedParam));
    }

    [TestCase("Invalid")]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    [Description("Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, invalidOrientation));
        Assert.That(ex.ParamName, Is.EqualTo("orientation"));
    }

    [TestCase("North")]
    [TestCase("South")]
    [TestCase("East")]
    [TestCase("West")]
    [Description("Verify that creating a LearningComponent with valid orientations succeeds")]
    public void Constructor_ValidOrientation_SetsOrientationCorrectly(string validOrientation)
    {
        var component = new LearningComponent(_componentId, _learningSpaceId, _width, _height, _depth, _x, _y, _z, validOrientation);
        Assert.That(component.Orientation, Is.EqualTo(validOrientation));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds")]
    public void Constructor_ZeroValues_CreatesEntityWithZeroProperties()
    {
        var component = new LearningComponent(_componentId, _learningSpaceId, 0f, 0f, 0f, 0f, 0f, 0f, _orientation);

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
