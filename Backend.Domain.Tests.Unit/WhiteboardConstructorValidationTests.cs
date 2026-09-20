using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="Whiteboard"/> constructor validation specific to
/// marker color validation and boundary values for story CPD-LC-001-005.
/// Covers intents Domain-001 through Domain-011.
/// </summary>
[TestFixture]
public class WhiteboardConstructorValidationTests
{
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    /// <summary>
    /// Domain-001: Verify that a Whiteboard can be created with a valid markerColor
    /// and all required LearningComponent properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Successfully create Whiteboard with valid markerColor and all properties")]
    public void Constructor_ValidMarkerColorAndProperties_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var markerColor = ValidMarkerColor;

        // Act
        var whiteboard = new Whiteboard(
            componentId, learningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.ComponentId, Is.EqualTo(componentId));
            Assert.That(whiteboard.MarkerColor, Is.EqualTo(markerColor));
            Assert.That(whiteboard.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(whiteboard.Width, Is.EqualTo(ValidWidth));
            Assert.That(whiteboard.Height, Is.EqualTo(ValidHeight));
            Assert.That(whiteboard.Depth, Is.EqualTo(ValidDepth));
            Assert.That(whiteboard.X, Is.EqualTo(ValidX));
            Assert.That(whiteboard.Y, Is.EqualTo(ValidY));
            Assert.That(whiteboard.Z, Is.EqualTo(ValidZ));
            Assert.That(whiteboard.Orientation, Is.EqualTo(ValidOrientation));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a Whiteboard with null or empty markerColor
    /// throws ArgumentException with parameter name "markerColor".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null markerColor throws ArgumentException")]
    [TestCase("", Description = "Domain-002: Empty markerColor throws ArgumentException")]
    public void Constructor_NullOrEmptyMarkerColor_ThrowsArgumentException(string? invalidMarkerColor)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, invalidMarkerColor!));

        Assert.That(ex!.ParamName, Is.EqualTo("markerColor"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a Whiteboard with an invalid color name
    /// (not in the valid color set) throws ArgumentException.
    /// </summary>
    [TestCase("NeonPink", Description = "Domain-003: Invalid color 'NeonPink' throws ArgumentException")]
    [TestCase("Purple", Description = "Domain-003: Invalid color 'Purple' throws ArgumentException")]
    [TestCase("Orange", Description = "Domain-003: Invalid color 'Orange' throws ArgumentException")]
    public void Constructor_InvalidColorName_ThrowsArgumentException(string invalidMarkerColor)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, invalidMarkerColor));

        Assert.That(ex!.Message, Does.Contain("Invalid color name"));
    }

    /// <summary>
    /// Domain-004 through Domain-009: Verify that creating a Whiteboard with a negative
    /// dimension or coordinate throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(-1f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-004: Negative width throws ArgumentException")]
    [TestCase(ValidWidth, -1f, ValidDepth, ValidX, ValidY, ValidZ, "height",
        Description = "Domain-005: Negative height throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, -0.1f, ValidX, ValidY, ValidZ, "depth",
        Description = "Domain-006: Negative depth throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, -1f, ValidY, ValidZ, "x",
        Description = "Domain-007: Negative X position throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, -1f, ValidZ, "y",
        Description = "Domain-008: Negative Y position throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1f, "z",
        Description = "Domain-009: Negative Z position throws ArgumentException")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth, float x, float y, float z, string expectedParamName)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                width, height, depth, x, y, z,
                ValidOrientation, ValidMarkerColor));

        Assert.That(ex!.ParamName, Is.EqualTo(expectedParamName));
    }

    /// <summary>
    /// Domain-010: Verify that creating a Whiteboard with an invalid orientation
    /// throws ArgumentException with parameter name "orientation".
    /// </summary>
    [TestCase("Northeast", Description = "Domain-010: Invalid orientation 'Northeast'")]
    [TestCase("Up", Description = "Domain-010: Invalid orientation 'Up'")]
    [TestCase("Down", Description = "Domain-010: Invalid orientation 'Down'")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                invalidOrientation, ValidMarkerColor));

        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions (boundary value)
    /// is accepted and all dimension properties are set to zero.
    /// </summary>
    [Test]
    [Description("Domain-011: Whiteboard with zero dimensions is accepted (boundary value)")]
    public void Constructor_ZeroDimensions_AllDimensionsAreZero()
    {
        // Arrange
        var zeroWidth = 0.0f;
        var zeroHeight = 0.0f;
        var zeroDepth = 0.0f;
        var zeroX = 0.0f;
        var zeroY = 0.0f;
        var zeroZ = 0.0f;

        // Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            zeroWidth, zeroHeight, zeroDepth,
            zeroX, zeroY, zeroZ,
            ValidOrientation, ValidMarkerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.Width, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Height, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Depth, Is.EqualTo(0.0f));
            Assert.That(whiteboard.X, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Y, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Z, Is.EqualTo(0.0f));
        });
    }
}
