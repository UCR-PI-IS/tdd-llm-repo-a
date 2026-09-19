using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity update functionality
/// and extended constructor validation introduced by the update whiteboard story.
/// Covers intents Domain-001 through Domain-014.
/// </summary>
[TestFixture]
public class WhiteboardUpdateTests
{
    // Valid test data constants
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    /// <summary>
    /// Domain-001: Verify that a Whiteboard can be created with valid markerColor
    /// and all required properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Successfully create a Whiteboard entity with valid data including marker color")]
    public void Constructor_ValidParametersAndMarkerColor_AllPropertiesSetCorrectly()
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
    /// Domain-003: Verify that creating a Whiteboard with an invalid color name
    /// (not in the valid color set) throws ArgumentException containing "Invalid color name".
    /// </summary>
    [Test]
    [Description("Domain-003: Invalid marker color name throws ArgumentException")]
    public void Constructor_InvalidMarkerColor_ThrowsArgumentException()
    {
        // Arrange
        var invalidMarkerColor = "NeonPink";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, invalidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Invalid color name"));
        });
    }

    /// <summary>
    /// Domain-004 to Domain-009: Verify that creating a Whiteboard with a negative
    /// dimension or coordinate throws an appropriate exception with the correct parameter name.
    /// </summary>
    [TestCase(-1.0f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-004: Negative width throws exception")]
    [TestCase(ValidWidth, -1.0f, ValidDepth, ValidX, ValidY, ValidZ, "height",
        Description = "Domain-005: Negative height throws exception")]
    [TestCase(ValidWidth, ValidHeight, -1.0f, ValidX, ValidY, ValidZ, "depth",
        Description = "Domain-006: Negative depth throws exception")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, -1.0f, ValidY, ValidZ, "x",
        Description = "Domain-007: Negative X position throws exception")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, -1.0f, ValidZ, "y",
        Description = "Domain-008: Negative Y position throws exception")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1.0f, "z",
        Description = "Domain-009: Negative Z position throws exception")]
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
    /// Domain-010: Verify that creating a Whiteboard with an invalid orientation
    /// throws ArgumentException with parameter name "orientation".
    /// </summary>
    [TestCase("Northeast", Description = "Domain-010: Invalid orientation 'Northeast' throws exception")]
    [TestCase("North", Description = "Domain-010: Invalid orientation 'North' throws exception")]
    [TestCase("Up", Description = "Domain-010: Invalid orientation 'Up' throws exception")]
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
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions
    /// (boundary value) is accepted.
    /// </summary>
    [Test]
    [Description("Domain-011: Zero dimensions and coordinates are accepted as boundary values")]
    public void Constructor_ZeroDimensionsBoundaryValues_Accepted()
    {
        // Arrange
        var width = 0.0f;
        var height = 0.0f;
        var depth = 0.0f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;

        // Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            width, height, depth, x, y, z,
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

    /// <summary>
    /// Domain-012: Verify that the Update method successfully updates
    /// all whiteboard properties with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates all whiteboard properties")]
    public void Update_ValidValues_UpdatesAllProperties()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        var newMarkerColor = "Red";
        var newWidth = 3.0f;
        var newHeight = 2.0f;
        var newDepth = 0.2f;
        var newX = 2.0f;
        var newY = 1.0f;
        var newZ = 3.0f;
        var newOrientation = "East";

        // Act
        whiteboard.Update(newWidth, newHeight, newDepth, newX, newY, newZ, newOrientation, newMarkerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.MarkerColor, Is.EqualTo(newMarkerColor));
            Assert.That(whiteboard.Width, Is.EqualTo(newWidth));
            Assert.That(whiteboard.Height, Is.EqualTo(newHeight));
            Assert.That(whiteboard.Depth, Is.EqualTo(newDepth));
            Assert.That(whiteboard.X, Is.EqualTo(newX));
            Assert.That(whiteboard.Y, Is.EqualTo(newY));
            Assert.That(whiteboard.Z, Is.EqualTo(newZ));
            Assert.That(whiteboard.Orientation, Is.EqualTo(newOrientation));
        });
    }

    /// <summary>
    /// Domain-013: Verify that the Update method throws InvalidOperationException
    /// when the new position exceeds the learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds space boundaries")]
    public void Update_PositionExceedsSpaceBoundaries_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        var learningSpaceWidth = 10.0f;
        var learningSpaceLength = 10.0f;
        var newX = 15.0f;
        var newZ = 15.0f;

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(
                ValidWidth, ValidHeight, ValidDepth,
                newX, ValidY, newZ,
                ValidOrientation, ValidMarkerColor,
                learningSpaceWidth, learningSpaceLength);
        }
        catch (InvalidOperationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected InvalidOperationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Position exceeds learning space boundaries"));
        });
    }

    /// <summary>
    /// Domain-014: Verify that the Update method throws InvalidOperationException
    /// when the new position overlaps with an existing component in the same learning space.
    /// </summary>
    [Test]
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with existing component")]
    public void Update_PositionOverlapsWithExistingComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        var existingComponents = new List<LearningComponent>
        {
            new Whiteboard(
                "WB-002", ValidLearningSpaceId,
                2.0f, 1.5f, 0.1f,
                3.0f, 0.0f, 4.0f,
                "East", "Green")
        };

        var newX = 3.0f;
        var newZ = 4.0f;

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(
                ValidWidth, ValidHeight, ValidDepth,
                newX, ValidY, newZ,
                ValidOrientation, ValidMarkerColor,
                existingComponents);
        }
        catch (InvalidOperationException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected InvalidOperationException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Position overlaps with existing component"));
        });
    }
}
