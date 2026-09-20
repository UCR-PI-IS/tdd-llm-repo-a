using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity update functionality and related constructor validation.
/// Covers intents Domain-001 through Domain-014 for CPD-LC-001-005.
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
    /// Domain-001: Verify that a Whiteboard can be created with valid markerColor and all required properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a Whiteboard can be created with valid markerColor and all required properties")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var markerColor = ValidMarkerColor;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act
        var whiteboard = new Whiteboard(
            componentId, learningSpaceId,
            width, height, depth,
            x, y, z,
            orientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.ComponentId, Is.EqualTo(componentId));
            Assert.That(whiteboard.MarkerColor, Is.EqualTo(markerColor));
            Assert.That(whiteboard.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(whiteboard.Width, Is.EqualTo(width));
            Assert.That(whiteboard.Height, Is.EqualTo(height));
            Assert.That(whiteboard.Depth, Is.EqualTo(depth));
            Assert.That(whiteboard.X, Is.EqualTo(x));
            Assert.That(whiteboard.Y, Is.EqualTo(y));
            Assert.That(whiteboard.Z, Is.EqualTo(z));
            Assert.That(whiteboard.Orientation, Is.EqualTo(orientation));
        });
    }

    /// <summary>
    /// Domain-002 and Domain-003: Verify that creating a Whiteboard with null, empty,
    /// or invalid markerColor throws ArgumentException.
    /// </summary>
    [TestCase(null, "markerColor",
        Description = "Domain-002: Null markerColor throws ArgumentException")]
    [TestCase("", "markerColor",
        Description = "Domain-002: Empty markerColor throws ArgumentException")]
    [TestCase("NeonPink", "Invalid color name",
        Description = "Domain-003: Invalid color name throws ArgumentException")]
    public void Constructor_InvalidMarkerColor_ThrowsArgumentException(
        string? invalidMarkerColor, string expectedContent)
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
            Assert.That(caughtException, Is.Not.Null,
                "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedContent));
        });
    }

    /// <summary>
    /// Domain-004 through Domain-009: Verify that creating a Whiteboard with a negative
    /// dimension or coordinate throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(-1.0f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-004: Negative width throws ArgumentException")]
    [TestCase(ValidWidth, -1.0f, ValidDepth, ValidX, ValidY, ValidZ, "height",
        Description = "Domain-005: Negative height throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, -0.1f, ValidX, ValidY, ValidZ, "depth",
        Description = "Domain-006: Negative depth throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, -1.0f, ValidY, ValidZ, "x",
        Description = "Domain-007: Negative X position throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, -1.0f, ValidZ, "y",
        Description = "Domain-008: Negative Y position throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1.0f, "z",
        Description = "Domain-009: Negative Z position throws ArgumentException")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth,
        float x, float y, float z, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                width, height, depth,
                x, y, z,
                ValidOrientation, ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null,
                "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo(expectedParamName));
        });
    }

    /// <summary>
    /// Domain-010: Verify that creating a Whiteboard with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-010: Invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                "Northeast", ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null,
                "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("orientation"));
        });
    }

    /// <summary>
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions (boundary value) is accepted.
    /// </summary>
    [Test]
    [Description("Domain-011: Zero dimensions (boundary value) are accepted")]
    public void Constructor_ZeroDimensions_AllDimensionPropertiesAreZero()
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
            width, height, depth,
            x, y, z,
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
    /// Domain-012: Verify that Update method successfully updates whiteboard properties with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates whiteboard properties with valid values")]
    public void Update_ValidParameters_UpdatesAllProperties()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        var newWidth = 3.0f;
        var newHeight = 2.0f;
        var newDepth = 0.2f;
        var newX = 2.0f;
        var newY = 1.0f;
        var newZ = 3.0f;
        var newOrientation = "South";
        var newMarkerColor = "Red";

        // Act
        whiteboard.Update(
            newWidth, newHeight, newDepth,
            newX, newY, newZ,
            newOrientation, newMarkerColor);

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
    /// Domain-013: Verify that Update method throws InvalidOperationException
    /// when new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds learning space boundaries")]
    public void Update_PositionExceedsLearningSpaceBoundaries_ThrowsInvalidOperationException()
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
            Assert.That(caughtException, Is.Not.Null,
                "Expected InvalidOperationException was not thrown");
            Assert.That(caughtException!.Message,
                Does.Contain("Position exceeds learning space boundaries"));
        });
    }

    /// <summary>
    /// Domain-014: Verify that Update method throws InvalidOperationException
    /// when new position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with another component")]
    public void Update_PositionOverlapsWithExistingComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        var otherComponent = new Whiteboard(
            "WB-002", ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            3.0f, 0.0f, 4.0f,
            "North", "Green");

        var existingComponents = new List<Whiteboard> { otherComponent };
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
            Assert.That(caughtException, Is.Not.Null,
                "Expected InvalidOperationException was not thrown");
            Assert.That(caughtException!.Message,
                Does.Contain("Position overlaps with existing component"));
        });
    }
}
