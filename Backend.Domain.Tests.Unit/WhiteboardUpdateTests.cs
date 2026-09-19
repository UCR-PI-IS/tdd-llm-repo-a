using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard"/> entity Update method and additional constructor validations.
/// Covers intents Domain-001 through Domain-014 for CPD-LC-001-005.
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
    /// Domain-001: Verify that a Whiteboard can be created with valid markerColor and all required LearningComponent properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Successfully create a Whiteboard with valid markerColor and all required properties")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var markerColor = "Blue";
        var width = 2.0f;
        var height = 1.5f;
        var depth = 0.1f;
        var x = 1.0f;
        var y = 0.0f;
        var z = 2.0f;
        var orientation = "North";

        // Act
        var whiteboard = new Whiteboard(componentId, learningSpaceId, width, height, depth, x, y, z, orientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.ComponentId, Is.EqualTo(componentId));
            Assert.That(whiteboard.MarkerColor, Is.EqualTo(markerColor));
            Assert.That(whiteboard.LearningSpaceId, Is.EqualTo(learningSpaceId));
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
    /// Domain-003: Verify that creating a Whiteboard with invalid color name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Invalid color name throws ArgumentException with appropriate message")]
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
            Assert.That(caughtException!.Message, Does.Contain("Invalid color name").Or.Contains("Invalid"));
        });
    }

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
                ValidComponentId, ValidLearningSpaceId,
                invalidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
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
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, invalidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
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
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, invalidDepth,
                ValidX, ValidY, ValidZ,
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
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                invalidX, ValidY, ValidZ,
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
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, invalidY, ValidZ,
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
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, invalidZ,
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
    /// Domain-011: Verify that creating a Whiteboard with zero dimensions (boundary value) is accepted.
    /// </summary>
    [Test]
    [Description("Domain-011: Zero dimensions are accepted as valid boundary values")]
    public void Constructor_ZeroDimensions_AcceptsBoundaryValues()
    {
        // Arrange
        var width = 0.0f;
        var height = 0.0f;
        var depth = 0.0f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var orientation = "North";
        var markerColor = "Blue";

        // Act
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            width, height, depth,
            x, y, z,
            orientation, markerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(whiteboard.Width, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Height, Is.EqualTo(0.0f));
            Assert.That(whiteboard.Depth, Is.EqualTo(0.0f));
        });
    }

    /// <summary>
    /// Domain-012: Verify that Update method successfully updates whiteboard properties with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates all whiteboard properties")]
    public void Update_ValidValues_UpdatesAllProperties()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

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
    /// Domain-013: Verify that Update method throws InvalidOperationException when new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds learning space boundaries")]
    public void Update_PositionExceedsBoundaries_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        var learningSpaceWidth = 10.0f;
        var learningSpaceLength = 10.0f;
        var newX = 15.0f;
        var newZ = 15.0f;

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(2.0f, 1.5f, 0.1f, newX, 0.0f, newZ, "North", "Blue", learningSpaceWidth, learningSpaceLength);
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
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with existing component")]
    public void Update_PositionOverlapsWithComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var whiteboard = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            1.0f, 0.0f, 2.0f,
            "North", "Blue");

        var existingComponents = new List<LearningComponent>
        {
            new Whiteboard("WB-002", ValidLearningSpaceId, 2.0f, 1.5f, 0.1f, 3.0f, 0.0f, 4.0f, "North", "Green")
        };

        var newX = 3.0f;
        var newZ = 4.0f;

        // Act
        InvalidOperationException? caughtException = null;
        try
        {
            whiteboard.Update(2.0f, 1.5f, 0.1f, newX, 0.0f, newZ, "North", "Blue", existingComponents);
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
