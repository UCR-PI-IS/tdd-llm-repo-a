using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity update operations
/// and additional constructor validation scenarios.
/// Covers intents Domain-003, Domain-011 through Domain-014 for CPD-LC-001-005.
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
    /// Domain-003: Verify that creating a Whiteboard with an invalid color name
    /// that is not in the valid color set throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Invalid color name throws ArgumentException")]
    public void Constructor_InvalidColorName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            _ = new Whiteboard(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                ValidOrientation, "NeonPink");
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
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions
    /// (boundary value) is accepted.
    /// </summary>
    [Test]
    [Description("Domain-011: Zero dimensions are accepted as boundary values")]
    public void Constructor_ZeroDimensions_SetsDimensionsToZero()
    {
        // Arrange & Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            0.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 0.0f,
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
    /// Domain-012: Verify that Update method successfully updates
    /// whiteboard properties with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method updates all properties with valid values")]
    public void Update_ValidParameters_UpdatesAllProperties()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        whiteboard.Update(
            3.0f, 2.0f, 0.2f,
            2.0f, 1.0f, 3.0f,
            "South", "Red");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.Width, Is.EqualTo(3.0f));
            Assert.That(whiteboard.Height, Is.EqualTo(2.0f));
            Assert.That(whiteboard.Depth, Is.EqualTo(0.2f));
            Assert.That(whiteboard.X, Is.EqualTo(2.0f));
            Assert.That(whiteboard.Y, Is.EqualTo(1.0f));
            Assert.That(whiteboard.Z, Is.EqualTo(3.0f));
            Assert.That(whiteboard.Orientation, Is.EqualTo("South"));
            Assert.That(whiteboard.MarkerColor, Is.EqualTo("Red"));
        });
    }

    /// <summary>
    /// Domain-013: Verify that Update method throws InvalidOperationException
    /// when new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds learning space boundaries")]
    public void Update_PositionExceedsBoundaries_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(
                2.0f, 1.5f, 0.1f,
                15.0f, 0.0f, 15.0f,
                ValidOrientation, ValidMarkerColor,
                10.0f, 10.0f);
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
    /// Domain-014: Verify that Update method throws InvalidOperationException
    /// when new position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with another component")]
    public void Update_PositionOverlaps_ThrowsInvalidOperationException()
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
                "South", "Green")
        };

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(
                2.0f, 1.5f, 0.1f,
                3.0f, 0.0f, 4.0f,
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
