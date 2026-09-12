using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructors and validation.
/// Covers intents Domain-001 through Domain-014.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    // Valid test data constants
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 1.5f;
    private const float ValidHeight = 1.0f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";
    private const string ExplicitComponentId = "COMP-12345";

    /// <summary>
    /// Domain-001: Verify that a LearningComponent without an explicit ID gets an auto-generated ID.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningComponent without an explicit ID gets an auto-generated ID")]
    public void Constructor_WithoutExplicitId_AutoGeneratesId()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, ValidOrientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.Not.Null);
            Assert.That(component.ComponentId, Is.Not.Empty);
        });
    }

    /// <summary>
    /// Domain-002: Verify that a LearningComponent with an explicit ID uses the provided ID.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify that a LearningComponent with an explicit ID uses the provided ID")]
    public void Constructor_WithExplicitId_UsesProvidedId()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ExplicitComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, ValidOrientation);

        // Assert
        Assert.That(component.ComponentId, Is.EqualTo(ExplicitComponentId));
    }

    /// <summary>
    /// Domain-003 to Domain-009 and Domain-004: Verify that creating a LearningComponent with
    /// negative dimension/coordinate or zero width throws the expected exception.
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
    public void Constructor_InvalidDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth, float x, float y, float z, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ExplicitComponentId, ValidLearningSpaceId,
                width, height, depth, x, y, z, ValidOrientation);
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
    /// Domain-010: Verify that creating a LearningComponent with an invalid orientation
    /// throws ArgumentException with parameter name "orientation".
    /// </summary>
    [TestCase("InvalidDirection", Description = "Domain-010: Invalid direction throws ArgumentException")]
    [TestCase("Northeast", Description = "Domain-010: Northeast throws ArgumentException")]
    [TestCase("", Description = "Domain-010: Empty orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ExplicitComponentId, ValidLearningSpaceId,
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
    /// Domain-011 to Domain-014: Verify that creating a LearningComponent with each valid
    /// orientation succeeds and the orientation is correctly assigned.
    /// </summary>
    [TestCase("North", Description = "Domain-011: Valid orientation North succeeds")]
    [TestCase("South", Description = "Domain-012: Valid orientation South succeeds")]
    [TestCase("East", Description = "Domain-013: Valid orientation East succeeds")]
    [TestCase("West", Description = "Domain-014: Valid orientation West succeeds")]
    public void Constructor_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            ExplicitComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }
}
