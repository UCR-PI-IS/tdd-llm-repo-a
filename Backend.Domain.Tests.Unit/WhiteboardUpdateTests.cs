using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity Update method.
/// Covers intents Domain-012 through Domain-014.
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

    /// <summary>
    /// Creates a valid whiteboard for testing.
    /// </summary>
    private static Whiteboard CreateValidWhiteboard()
    {
        return new Whiteboard(
            ValidComponentId,
            ValidLearningSpaceId,
            ValidWidth,
            ValidHeight,
            ValidDepth,
            ValidX,
            ValidY,
            ValidZ,
            ValidOrientation,
            ValidMarkerColor);
    }

    /// <summary>
    /// Domain-012: Verify that Update method successfully updates whiteboard properties with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates whiteboard properties with valid values")]
    public void Update_ValidValues_UpdatesAllProperties()
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
    /// Domain-013: Verify that Update method throws InvalidOperationException when new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update method throws InvalidOperationException when position exceeds learning space boundaries")]
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
            whiteboard.Update(2.0f, 1.5f, 0.1f, newX, 0.0f, newZ, "South", "Blue", learningSpaceWidth, learningSpaceLength);
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
    /// Domain-014: Verify that Update method throws InvalidOperationException when new position overlaps with another component.
    /// </summary>
    [Test]
    [Description("Domain-014: Update method throws InvalidOperationException when position overlaps with existing component")]
    public void Update_PositionOverlapsWithComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = CreateValidWhiteboard();
        var otherComponentId = "WB-002";
        var otherComponent = new Whiteboard(
            otherComponentId,
            ValidLearningSpaceId,
            2.0f,
            1.5f,
            0.1f,
            3.0f,
            0.0f,
            4.0f,
            "South",
            "Green");
        var existingComponents = new List<LearningComponent> { otherComponent };
        var newX = 3.0f;
        var newZ = 4.0f;

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(2.0f, 1.5f, 0.1f, newX, 0.0f, newZ, "South", "Blue", existingComponents);
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
