using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="Whiteboard"/> marker color validation.
/// Covers intents Domain-001 through Domain-003.
/// </summary>
[TestFixture]
public class WhiteboardMarkerColorValidationTests
{
    // Valid test data constants
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";

    /// <summary>
    /// Domain-001: Verify that a Whiteboard can be created with valid markerColor and all required LearningComponent properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Successfully create Whiteboard with valid markerColor and all properties")]
    public void Constructor_ValidMarkerColor_CreatesWhiteboardWithCorrectProperties()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var markerColor = "Blue";
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
        var orientation = ValidOrientation;

        // Act
        var whiteboard = new Whiteboard(
            componentId,
            learningSpaceId,
            width,
            height,
            depth,
            x,
            y,
            z,
            orientation,
            markerColor);

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
    /// Domain-002: Verify that creating a Whiteboard with null or empty markerColor throws ArgumentException.
    /// </summary>
    [TestCase("", Description = "Domain-002: Empty markerColor throws ArgumentException")]
    [TestCase(null, Description = "Domain-002: Null markerColor throws ArgumentException")]
    public void Constructor_NullOrEmptyMarkerColor_ThrowsArgumentException(string? invalidMarkerColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId,
                ValidLearningSpaceId,
                ValidWidth,
                ValidHeight,
                ValidDepth,
                ValidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                invalidMarkerColor!);
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
    /// Domain-003: Verify that creating a Whiteboard with invalid color name (not in valid color set) throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Invalid color name throws ArgumentException")]
    public void Constructor_InvalidColorName_ThrowsArgumentException()
    {
        // Arrange
        var invalidMarkerColor = "NeonPink";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId,
                ValidLearningSpaceId,
                ValidWidth,
                ValidHeight,
                ValidDepth,
                ValidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                invalidMarkerColor);
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
}
