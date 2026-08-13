using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor and validation.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    // --- Valid default values used across tests ---
    private const string ValidComponentId = "LC-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 2.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Verifies that a LearningComponent entity can be created with valid parameters
    /// and all properties are set correctly.
    /// </summary>
    [Test(Description = "Domain-001: Constructor with valid parameters sets all properties")]
    public void Constructor_ValidParameters_SetsAllProperties()
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
    /// Verifies that creating a LearningComponent with a negative width throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-002: Constructor with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var invalidWidth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                invalidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative height throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-003: Constructor with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var invalidHeight = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, invalidHeight, ValidDepth,
                ValidX, ValidY, ValidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative depth throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-004: Constructor with negative depth throws ArgumentException")]
    public void Constructor_NegativeDepth_ThrowsArgumentException()
    {
        // Arrange
        var invalidDepth = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, invalidDepth,
                ValidX, ValidY, ValidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("depth"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative X coordinate throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-005: Constructor with negative X coordinate throws ArgumentException")]
    public void Constructor_NegativeX_ThrowsArgumentException()
    {
        // Arrange
        var invalidX = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                invalidX, ValidY, ValidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("x"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative Y coordinate throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-006: Constructor with negative Y coordinate throws ArgumentException")]
    public void Constructor_NegativeY_ThrowsArgumentException()
    {
        // Arrange
        var invalidY = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, invalidY, ValidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("y"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with a negative Z coordinate throws ArgumentException.
    /// </summary>
    [Test(Description = "Domain-007: Constructor with negative Z coordinate throws ArgumentException")]
    public void Constructor_NegativeZ_ThrowsArgumentException()
    {
        // Arrange
        var invalidZ = -1.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, invalidZ, ValidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("z"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with an invalid orientation throws ArgumentException.
    /// </summary>
    [TestCase("Northeast", Description = "Domain-008: Invalid orientation 'Northeast' throws ArgumentException")]
    [TestCase("Up", Description = "Domain-008: Invalid orientation 'Up' throws ArgumentException")]
    [TestCase("", Description = "Domain-008: Empty orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ, invalidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with each valid orientation succeeds.
    /// </summary>
    [TestCase("North", Description = "Domain-009: Valid orientation 'North'")]
    [TestCase("South", Description = "Domain-009: Valid orientation 'South'")]
    [TestCase("East", Description = "Domain-009: Valid orientation 'East'")]
    [TestCase("West", Description = "Domain-009: Valid orientation 'West'")]
    public void Constructor_ValidOrientation_SetsOrientation(string orientation)
    {
        // Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Verifies that creating a LearningComponent with zero values for dimensions
    /// and coordinates succeeds (boundary test).
    /// </summary>
    [Test(Description = "Domain-010: Constructor with zero values for dimensions and coordinates succeeds")]
    public void Constructor_ZeroValues_SetsAllPropertiesToZero()
    {
        // Arrange
        var zero = 0f;

        // Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            zero, zero, zero,
            zero, zero, zero, ValidOrientation);

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
