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
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.0f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.5f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    /// <summary>
    /// Domain-001: Verify that a Whiteboard entity can be created with valid parameters
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Successfully create a Whiteboard entity with valid data")]
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
                ValidComponentId, ValidLearningSpaceId,
                width, height, depth, x, y, z,
                ValidOrientation, ValidMarkerColor);
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
    [TestCase("North", Description = "Domain-008: Invalid orientation 'North' for whiteboard")]
    [TestCase("Northeast", Description = "Domain-008: Invalid orientation 'Northeast'")]
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
    /// Domain-009: Verify that creating a Whiteboard with a null or empty markerColor
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
    /// Domain-010: Verify that creating a Whiteboard with zero position coordinates
    /// succeeds and the position properties are set to zero (valid boundary).
    /// </summary>
    [Test]
    [Description("Domain-010: Successfully create Whiteboard with zero position coordinates (edge case)")]
    public void Constructor_ZeroPositionCoordinates_PropertiesSetToZero()
    {
        // Arrange
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;

        // Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            x, y, z,
            ValidOrientation, ValidMarkerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.X, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Y, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Z, Is.EqualTo(0.0f));
        });
    }

    /// <summary>
    /// Domain-011: Verify that creating a Whiteboard with each valid orientation
    /// (South, East, West) succeeds and the orientation is correctly assigned.
    /// </summary>
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
    /// Test case source for FitsInSpace scenarios where the whiteboard fits within the space.
    /// Covers Domain-012 (fits within) and Domain-016 (exact match boundary).
    /// </summary>
    public static IEnumerable<TestCaseData> FitsWithinSpaceCases
    {
        get
        {
            yield return new TestCaseData(
                2.0f, 1.0f, 0.5f, 1.0f, 0.5f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-012: Whiteboard fits within learning space dimensions");

            yield return new TestCaseData(
                8.0f, 3.0f, 10.0f, 0.0f, 0.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-016: Whiteboard dimensions exactly match learning space (boundary)");
        }
    }

    /// <summary>
    /// Domain-012 and Domain-016: Verify that FitsInSpace returns true when the whiteboard
    /// dimensions fit within or exactly match the learning space dimensions.
    /// </summary>
    [TestCaseSource(nameof(FitsWithinSpaceCases))]
    public void FitsInSpace_WhiteboardFitsWithinSpace_ReturnsTrue(
        float wbWidth, float wbHeight, float wbDepth, float wbX, float wbY, float wbZ,
        float spaceHeight, float spaceWidth, float spaceLength)
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            wbWidth, wbHeight, wbDepth, wbX, wbY, wbZ,
            ValidOrientation, ValidMarkerColor);
        var learningSpace = new LearningSpace("Classroom", spaceHeight, spaceWidth, spaceLength);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.True);
    }

    /// <summary>
    /// Test case source for FitsInSpace scenarios where a whiteboard dimension exceeds
    /// the corresponding learning space dimension.
    /// Covers Domain-013 (width), Domain-014 (height), Domain-015 (depth/length).
    /// </summary>
    public static IEnumerable<TestCaseData> DimensionExceedsSpaceCases
    {
        get
        {
            yield return new TestCaseData(
                10.0f, 1.0f, 0.5f, 0.0f, 0.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-013: Whiteboard width exceeds learning space width");

            yield return new TestCaseData(
                2.0f, 5.0f, 0.5f, 0.0f, 0.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-014: Whiteboard height exceeds learning space height");

            yield return new TestCaseData(
                2.0f, 1.0f, 15.0f, 0.0f, 0.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-015: Whiteboard depth exceeds learning space length");
        }
    }

    /// <summary>
    /// Domain-013, Domain-014, Domain-015: Verify that FitsInSpace returns false when
    /// a whiteboard dimension exceeds the corresponding learning space dimension.
    /// </summary>
    [TestCaseSource(nameof(DimensionExceedsSpaceCases))]
    public void FitsInSpace_DimensionExceedsSpaceDimension_ReturnsFalse(
        float wbWidth, float wbHeight, float wbDepth, float wbX, float wbY, float wbZ,
        float spaceHeight, float spaceWidth, float spaceLength)
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            wbWidth, wbHeight, wbDepth, wbX, wbY, wbZ,
            ValidOrientation, ValidMarkerColor);
        var learningSpace = new LearningSpace("Classroom", spaceHeight, spaceWidth, spaceLength);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }

    /// <summary>
    /// Test case source for FitsInSpace scenarios where the whiteboard position plus
    /// a dimension exceeds the corresponding learning space dimension.
    /// Covers Domain-017 (X+width), Domain-018 (Z+depth), Domain-019 (Y+height).
    /// </summary>
    public static IEnumerable<TestCaseData> PositionPlusDimensionExceedsCases
    {
        get
        {
            yield return new TestCaseData(
                5.0f, 1.0f, 0.5f, 5.0f, 0.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-017: X position + width exceeds learning space width (5+5=10 > 8)");

            yield return new TestCaseData(
                2.0f, 1.0f, 5.0f, 0.0f, 0.0f, 7.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-018: Z position + depth exceeds learning space length (7+5=12 > 10)");

            yield return new TestCaseData(
                2.0f, 2.0f, 0.5f, 0.0f, 2.0f, 0.0f,
                3.0f, 8.0f, 10.0f
            ).SetDescription("Domain-019: Y position + height exceeds learning space height (2+2=4 > 3)");
        }
    }

    /// <summary>
    /// Domain-017, Domain-018, Domain-019: Verify that FitsInSpace returns false when
    /// the whiteboard position plus a dimension exceeds the corresponding learning space dimension.
    /// </summary>
    [TestCaseSource(nameof(PositionPlusDimensionExceedsCases))]
    public void FitsInSpace_PositionPlusDimensionExceedsSpace_ReturnsFalse(
        float wbWidth, float wbHeight, float wbDepth, float wbX, float wbY, float wbZ,
        float spaceHeight, float spaceWidth, float spaceLength)
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            wbWidth, wbHeight, wbDepth, wbX, wbY, wbZ,
            ValidOrientation, ValidMarkerColor);
        var learningSpace = new LearningSpace("Classroom", spaceHeight, spaceWidth, spaceLength);

        // Act
        var fits = whiteboard.FitsInSpace(learningSpace);

        // Assert
        Assert.That(fits, Is.False);
    }
}
