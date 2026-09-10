using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor overloads
/// introduced by story CPD-LC-001-009 (auto-generated ID and updated validation).
/// Covers intents Domain-001 through Domain-014.
/// </summary>
[TestFixture]
public class LearningComponentCreationTests
{
    // Valid test data constants for the constructor without componentId
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 1.5f;
    private const float ValidHeight = 1.0f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Domain-001: Verify that a LearningComponent can be created with an auto-generated ID
    /// when no ID is provided via the constructor overload without componentId.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningComponent can be created with an auto-generated ID when no ID is provided")]
    public void Constructor_WithoutComponentId_AutoGeneratesId()
    {
        // Arrange & Act
        var component = new LearningComponent(
            learningSpaceId: ValidLearningSpaceId,
            width: ValidWidth,
            height: ValidHeight,
            depth: ValidDepth,
            x: ValidX,
            y: ValidY,
            z: ValidZ,
            orientation: ValidOrientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.Not.Null);
            Assert.That(component.ComponentId, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Domain-002: Verify that a LearningComponent can be created with an explicitly provided ID
    /// using the full constructor, and the provided ID is preserved.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify that a LearningComponent can be created with an explicitly provided ID")]
    public void Constructor_WithExplicitComponentId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-12345";

        // Act
        var component = new LearningComponent(
            componentId: explicitId,
            learningSpaceId: ValidLearningSpaceId,
            width: ValidWidth,
            height: ValidHeight,
            depth: ValidDepth,
            x: ValidX,
            y: ValidY,
            z: ValidZ,
            orientation: ValidOrientation);

        // Assert
        Assert.That(component.ComponentId, Is.EqualTo(explicitId));
    }

    /// <summary>
    /// Domain-003 through Domain-009: Verify that creating a LearningComponent (without explicit ID)
    /// with a negative or zero dimension, or a negative coordinate, throws ArgumentException
    /// with the correct parameter name.
    /// </summary>
    [TestCase(-1.5f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-003: Negative width throws ArgumentException")]
    [TestCase(0.0f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width",
        Description = "Domain-004: Zero width throws ArgumentException")]
    [TestCase(ValidWidth, -1.0f, ValidDepth, ValidX, ValidY, ValidZ, "height",
        Description = "Domain-005: Negative height throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, -0.5f, ValidX, ValidY, ValidZ, "depth",
        Description = "Domain-006: Negative depth throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, -10.0f, ValidY, ValidZ, "x",
        Description = "Domain-007: Negative X coordinate throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, -5.0f, ValidZ, "y",
        Description = "Domain-008: Negative Y coordinate throws ArgumentException")]
    [TestCase(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1.0f, "z",
        Description = "Domain-009: Negative Z coordinate throws ArgumentException")]
    public void Constructor_WithoutId_InvalidDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth, float x, float y, float z, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                learningSpaceId: ValidLearningSpaceId,
                width: width,
                height: height,
                depth: depth,
                x: x,
                y: y,
                z: z,
                orientation: ValidOrientation);
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
    /// Domain-010: Verify that creating a LearningComponent (without explicit ID)
    /// with an invalid orientation throws ArgumentException with parameter name "orientation".
    /// </summary>
    [Test]
    [Description("Domain-010: Verify that creating a LearningComponent with invalid orientation throws ArgumentException")]
    public void Constructor_WithoutId_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                learningSpaceId: ValidLearningSpaceId,
                width: ValidWidth,
                height: ValidHeight,
                depth: ValidDepth,
                x: ValidX,
                y: ValidY,
                z: ValidZ,
                orientation: "InvalidDirection");
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
    /// Domain-011 through Domain-014: Verify that creating a LearningComponent (without explicit ID)
    /// with each valid orientation (North, South, East, West) succeeds and the orientation
    /// is correctly assigned.
    /// </summary>
    [TestCase("North", Description = "Domain-011: Valid orientation North succeeds")]
    [TestCase("South", Description = "Domain-012: Valid orientation South succeeds")]
    [TestCase("East", Description = "Domain-013: Valid orientation East succeeds")]
    [TestCase("West", Description = "Domain-014: Valid orientation West succeeds")]
    public void Constructor_WithoutId_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            learningSpaceId: ValidLearningSpaceId,
            width: ValidWidth,
            height: ValidHeight,
            depth: ValidDepth,
            x: ValidX,
            y: ValidY,
            z: ValidZ,
            orientation: orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }
}
