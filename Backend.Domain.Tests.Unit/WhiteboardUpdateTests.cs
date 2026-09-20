using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity update functionality
/// and new constructor validation introduced by story CPD-LC-001-005.
/// Covers intents Domain-003, Domain-011, Domain-012, Domain-013, Domain-014.
/// </summary>
[TestFixture]
public class WhiteboardUpdateTests
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
    private const string ValidMarkerColor = "Blue";

    private static Whiteboard CreateValidWhiteboard()
    {
        return new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    /// <summary>
    /// Domain-003: Verify that creating a Whiteboard with an invalid color name
    /// (not in the valid color set) throws ArgumentException with a message
    /// containing "Invalid color name".
    /// </summary>
    [Test]
    [Description("Domain-003: Constructor throws ArgumentException for invalid color name not in valid set")]
    public void Constructor_InvalidColorName_ThrowsArgumentException()
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
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions (boundary value)
    /// is accepted and all dimension properties are set to zero.
    /// </summary>
    [Test]
    [Description("Domain-011: Constructor accepts zero dimensions as valid boundary values")]
    public void Constructor_ZeroDimensions_AllDimensionsAreZero()
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
        });
    }

    /// <summary>
    /// Domain-012: Verify that the Update method successfully updates all whiteboard
    /// properties when valid values are provided.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates all whiteboard properties with valid values")]
    public void Update_ValidValues_AllPropertiesUpdated()
    {
        // Arrange
        var whiteboard = CreateValidWhiteboard();
        var newMarkerColor = "Red";
        var newWidth = 3.0f;
        var newHeight = 2.0f;
        var newDepth = 0.2f;
        var newX = 2.0f;
        var newY = 1.0f;
        var newZ = 3.0f;
        var newOrientation = "South";

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
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds learning space boundaries")]
    public void Update_PositionExceedsBoundaries_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = CreateValidWhiteboard();
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
    /// when the new position overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with existing component")]
    public void Update_PositionOverlapsWithExistingComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = CreateValidWhiteboard();
        var existingComponents = new List<Whiteboard>
        {
            new Whiteboard(
                "WB-002", ValidLearningSpaceId,
                2.0f, 1.5f, 0.1f,
                3.0f, 0.0f, 4.0f,
                "South", "Green")
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
