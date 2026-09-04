using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity constructor, validation, and FitsInSpace method.
/// Covers intents Domain-001 through Domain-019.
/// </summary>
[TestFixture]
public class WhiteboardTests
{
    // Valid test data constants
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 20.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";
    private const string ValidMarkerColor = "Blue";

    /// <summary>
    /// Domain-001: Verify that a Whiteboard entity can be created with valid parameters
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a Whiteboard entity can be created with valid parameters")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
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
        var markerColor = ValidMarkerColor;

        // Act
        var whiteboard = new Whiteboard(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.ComponentId, Is.EqualTo(componentId));
            Assert.That(whiteboard.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(whiteboard.Width, Is.EqualTo(width));
            Assert.That(whiteboard.Height, Is.EqualTo(height));
            Assert.That(whiteboard.Depth, Is.EqualTo(depth));
            Assert.That(whiteboard.X, Is.EqualTo(x));
            Assert.That(whiteboard.Y, Is.EqualTo(y));
            Assert.That(whiteboard.Z, Is.EqualTo(z));
            Assert.That(whiteboard.Orientation, Is.EqualTo(orientation));
            Assert.That(whiteboard.MarkerColor, Is.EqualTo(markerColor));
        });
    }

    /// <summary>
    /// Domain-002 to Domain-007: Verify that creating a Whiteboard with a negative
    /// dimension or coordinate throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(-1f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-002: Negative width throws ArgumentException")]
    [TestCase(ValidWidth, -1f, ValidDepth, ValidX, ValidY, ValidZ, "height",
        Description = "Domain-003: Negative height throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, -1f, ValidX, ValidY, ValidZ, "depth",
        Description = "Domain-004: Negative depth throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, -1f, ValidY, ValidZ, "x",
        Description = "Domain-005: Negative X coordinate throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, -1f, ValidZ, "y",
        Description = "Domain-006: Negative Y coordinate throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1f, "z",
        Description = "Domain-007: Negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth, float x, float y, float z, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId, width, height, depth, x, y, z, ValidOrientation, ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(expectedParamName));
        });
    }

    /// <summary>
    /// Domain-008: Verify that creating a Whiteboard with an invalid orientation
    /// throws ArgumentException with parameter name "orientation".
    /// </summary>
    [TestCase("Northeast", Description = "Domain-008: Invalid orientation 'Northeast'")]
    [TestCase("Up", Description = "Domain-008: Invalid orientation 'Up'")]
    [TestCase("", Description = "Domain-008: Empty orientation string")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                invalidOrientation, ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("orientation"));
        });
    }

    /// <summary>
    /// Domain-009: Verify that creating a Whiteboard with null or empty markerColor
    /// throws ArgumentException with parameter name "markerColor".
    /// </summary>
    [TestCase(null, Description = "Domain-009: Null markerColor throws ArgumentException")]
    [TestCase("", Description = "Domain-009: Empty markerColor throws ArgumentException")]
    public void Constructor_NullOrEmptyMarkerColor_ThrowsArgumentException(string? invalidMarkerColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, invalidMarkerColor!);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("markerColor"));
        });
    }

    /// <summary>
    /// Domain-010: Verify that creating a Whiteboard with zero values for all
    /// position coordinates succeeds (boundary test).
    /// </summary>
    [Test]
    [Description("Domain-010: Verify that zero values for position coordinates succeed (boundary test)")]
    public void Constructor_ZeroPositionCoordinates_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = 0f;
        var y = 0f;
        var z = 0f;
        var orientation = ValidOrientation;
        var markerColor = ValidMarkerColor;

        // Act
        var whiteboard = new Whiteboard(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.X, Is.EqualTo(0f));
            Assert.That(whiteboard.Y, Is.EqualTo(0f));
            Assert.That(whiteboard.Z, Is.EqualTo(0f));
        });
    }

    /// <summary>
    /// Domain-011: Verify that creating a Whiteboard with each valid orientation
    /// (North, South, East, West) succeeds and the orientation is correctly assigned.
    /// </summary>
    [TestCase("North", Description = "Domain-011: Valid orientation North")]
    [TestCase("South", Description = "Domain-011: Valid orientation South")]
    [TestCase("East", Description = "Domain-011: Valid orientation East")]
    [TestCase("West", Description = "Domain-011: Valid orientation West")]
    public void Constructor_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange & Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            orientation, ValidMarkerColor);

        // Assert
        Assert.That(whiteboard.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Domain-012: Verify that FitsInSpace returns true when whiteboard dimensions
    /// fit within learning space dimensions.
    /// </summary>
    [Test]
    [Description("Domain-012: FitsInSpace returns true when whiteboard fits in learning space")]
    public void FitsInSpace_WhiteboardFits_ReturnsTrue()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.0f, 0.5f,  // Smaller than space
            1.0f, 1.0f, 0.0f,  // Position within bounds
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.True);
    }

    /// <summary>
    /// Domain-013: Verify that FitsInSpace returns false when whiteboard width
    /// exceeds learning space width.
    /// </summary>
    [Test]
    [Description("Domain-013: FitsInSpace returns false when whiteboard width exceeds space width")]
    public void FitsInSpace_WidthExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            15.0f, 1.0f, 0.5f,  // Width exceeds space width
            0.0f, 0.0f, 0.0f,
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Domain-014: Verify that FitsInSpace returns false when whiteboard height
    /// exceeds learning space height.
    /// </summary>
    [Test]
    [Description("Domain-014: FitsInSpace returns false when whiteboard height exceeds space height")]
    public void FitsInSpace_HeightExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 10.0f, 0.5f,  // Height exceeds space height
            0.0f, 0.0f, 0.0f,
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Domain-015: Verify that FitsInSpace returns false when whiteboard depth
    /// exceeds learning space length.
    /// </summary>
    [Test]
    [Description("Domain-015: FitsInSpace returns false when whiteboard depth exceeds space length")]
    public void FitsInSpace_DepthExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.0f, 20.0f,  // Depth exceeds space length
            0.0f, 0.0f, 0.0f,
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Domain-016: Verify that FitsInSpace returns true when whiteboard dimensions
    /// exactly match learning space dimensions (boundary test).
    /// </summary>
    [Test]
    [Description("Domain-016: FitsInSpace returns true when dimensions exactly match (boundary)")]
    public void FitsInSpace_ExactDimensions_ReturnsTrue()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            10.0f, 5.0f, 15.0f,  // Exact match
            0.0f, 0.0f, 0.0f,
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.True);
    }

    /// <summary>
    /// Domain-017: Verify that FitsInSpace returns false when whiteboard position
    /// plus width exceeds learning space width.
    /// </summary>
    [Test]
    [Description("Domain-017: FitsInSpace returns false when X + width exceeds space width")]
    public void FitsInSpace_PositionPlusWidthExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            5.0f, 1.0f, 0.5f,
            6.0f, 0.0f, 0.0f,  // X + width = 11 > 10
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Domain-018: Verify that FitsInSpace returns false when whiteboard position
    /// plus depth exceeds learning space length.
    /// </summary>
    [Test]
    [Description("Domain-018: FitsInSpace returns false when Z + depth exceeds space length")]
    public void FitsInSpace_PositionPlusDepthExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.0f, 10.0f,
            0.0f, 0.0f, 6.0f,  // Z + depth = 16 > 15
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Domain-019: Verify that FitsInSpace returns false when whiteboard Y position
    /// plus height exceeds learning space height.
    /// </summary>
    [Test]
    [Description("Domain-019: FitsInSpace returns false when Y + height exceeds space height")]
    public void FitsInSpace_PositionPlusHeightExceeds_ReturnsFalse()
    {
        // Arrange
        var learningSpace = new LearningSpace("Classroom", 5.0f, 10.0f, 15.0f);
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 3.0f, 0.5f,
            0.0f, 3.0f, 0.0f,  // Y + height = 6 > 5
            ValidOrientation, ValidMarkerColor);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }
}
