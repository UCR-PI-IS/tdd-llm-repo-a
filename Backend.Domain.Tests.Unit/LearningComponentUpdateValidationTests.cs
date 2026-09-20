using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponent"/> constructor validation.
/// Covers intents Domain-004 through Domain-011.
/// </summary>
[TestFixture]
public class LearningComponentUpdateValidationTests
{
    // Valid test data constants
    private const string ValidComponentId = "LC-001";
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
    /// Domain-004: Verify that creating a Whiteboard with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var invalidWidth = -1.0f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId,
                ValidLearningSpaceId,
                invalidWidth,
                ValidHeight,
                ValidDepth,
                ValidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("width"));
        });
    }

    /// <summary>
    /// Domain-005: Verify that creating a Whiteboard with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var invalidHeight = -1.0f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId,
                ValidLearningSpaceId,
                ValidWidth,
                invalidHeight,
                ValidDepth,
                ValidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("height"));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Whiteboard with negative depth throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        var invalidDepth = -0.1f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Whiteboard(
                ValidComponentId,
                ValidLearningSpaceId,
                ValidWidth,
                ValidHeight,
                invalidDepth,
                ValidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("depth"));
        });
    }

    /// <summary>
    /// Domain-007: Verify that creating a Whiteboard with negative X position throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Negative X position throws ArgumentException")]
    public void Constructor_NegativeXPosition_ThrowsArgumentException()
    {
        // Arrange
        var invalidX = -1.0f;

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
                invalidX,
                ValidY,
                ValidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("x"));
        });
    }

    /// <summary>
    /// Domain-008: Verify that creating a Whiteboard with negative Y position throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Negative Y position throws ArgumentException")]
    public void Constructor_NegativeYPosition_ThrowsArgumentException()
    {
        // Arrange
        var invalidY = -1.0f;

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
                invalidY,
                ValidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("y"));
        });
    }

    /// <summary>
    /// Domain-009: Verify that creating a Whiteboard with negative Z position throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-009: Negative Z position throws ArgumentException")]
    public void Constructor_NegativeZPosition_ThrowsArgumentException()
    {
        // Arrange
        var invalidZ = -1.0f;

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
                invalidZ,
                ValidOrientation,
                ValidMarkerColor);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("z"));
        });
    }

    /// <summary>
    /// Domain-010: Verify that creating a Whiteboard with invalid orientation throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-010: Invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var invalidOrientation = "Northeast";

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
                invalidOrientation,
                ValidMarkerColor);
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
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions (boundary value) is accepted.
    /// </summary>
    [Test]
    [Description("Domain-011: Zero dimensions are accepted as valid boundary values")]
    public void Constructor_ZeroDimensions_AcceptsZeroValues()
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
            ValidComponentId,
            ValidLearningSpaceId,
            zeroWidth,
            zeroHeight,
            zeroDepth,
            zeroX,
            zeroY,
            zeroZ,
            ValidOrientation,
            ValidMarkerColor);

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
