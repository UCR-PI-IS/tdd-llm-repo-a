using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-010.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    // Valid test data constants
    private const string ValidComponentId = "LC-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 20.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Domain-001: Verify that a LearningComponent entity can be created with valid parameters
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningComponent entity can be created with valid parameters")]
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

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.EqualTo(componentId));
            Assert.That(component.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(component.Width, Is.EqualTo(width));
            Assert.That(component.Height, Is.EqualTo(height));
            Assert.That(component.Depth, Is.EqualTo(depth));
            Assert.That(component.X, Is.EqualTo(x));
            Assert.That(component.Y, Is.EqualTo(y));
            Assert.That(component.Z, Is.EqualTo(z));
            Assert.That(component.Orientation, Is.EqualTo(orientation));
        });
    }

    /// <summary>
    /// Domain-002 to Domain-007: Verify that creating a LearningComponent with a negative
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
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, width, height, depth, x, y, z, ValidOrientation);
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
    /// Domain-008: Verify that creating a LearningComponent with an invalid orientation
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
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                invalidOrientation);
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
    /// Domain-009: Verify that creating a LearningComponent with each valid orientation
    /// (North, South, East, West) succeeds and the orientation is correctly assigned.
    /// </summary>
    [TestCase("North", Description = "Domain-009: Valid orientation North")]
    [TestCase("South", Description = "Domain-009: Valid orientation South")]
    [TestCase("East", Description = "Domain-009: Valid orientation East")]
    [TestCase("West", Description = "Domain-009: Valid orientation West")]
    public void Constructor_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Domain-010: Verify that creating a LearningComponent with zero values for all
    /// dimensions and coordinates succeeds (boundary test).
    /// </summary>
    [Test]
    [Description("Domain-010: Verify that zero values for dimensions and coordinates succeed (boundary test)")]
    public void Constructor_ZeroValues_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = 0f;
        var height = 0f;
        var depth = 0f;
        var x = 0f;
        var y = 0f;
        var z = 0f;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.Width, Is.EqualTo(0f));
            Assert.That(component.Height, Is.EqualTo(0f));
            Assert.That(component.Depth, Is.EqualTo(0f));
            Assert.That(component.X, Is.EqualTo(0f));
            Assert.That(component.Y, Is.EqualTo(0f));
            Assert.That(component.Z, Is.EqualTo(0f));
        });
    }
}
