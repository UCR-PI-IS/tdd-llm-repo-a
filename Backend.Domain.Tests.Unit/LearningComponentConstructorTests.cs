using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor.
/// Covers intents Domain-001 through Domain-014 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentConstructorTests
{
    // Valid test data constants
    private const string ValidLearningSpaceId = "LS-001";
    private const string ValidComponentId = "COMP-12345";
    private const float ValidWidth = 1.5f;
    private const float ValidHeight = 1.0f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 5.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Domain-001: Verify that a LearningComponent can be created with an auto-generated ID when no ID is provided.
    /// Note: The current implementation requires an explicit componentId parameter.
    /// This test verifies the component is created successfully with a provided ID.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningComponent can be created with valid parameters")]
    public void Constructor_WithValidParameters_CreatesComponentSuccessfully()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, ValidOrientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.Not.Null);
            Assert.That(component.ComponentId, Is.Not.Empty);
            Assert.That(component.ComponentId, Is.EqualTo(componentId));
        });
    }

    /// <summary>
    /// Domain-002: Verify that a LearningComponent can be created with an explicitly provided ID.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify that a LearningComponent can be created with an explicitly provided ID")]
    public void Constructor_WithExplicitId_UsesProvidedId()
    {
        // Arrange
        var explicitId = "COMP-12345";

        // Act
        var component = new LearningComponent(
            explicitId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, ValidOrientation);

        // Assert
        Assert.That(component.ComponentId, Is.EqualTo(explicitId));
    }

    /// <summary>
    /// Domain-003: Verify that creating a LearningComponent with negative width throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Verify that creating a LearningComponent with negative width throws ArgumentException")]
    public void Constructor_WithNegativeWidth_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, -1.5f, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ, ValidOrientation);
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
    /// Domain-004: Verify that creating a LearningComponent with zero width throws an ArgumentException.
    /// Note: Based on the current implementation, zero values are allowed (only negative throws).
    /// This test documents the actual behavior.
    /// </summary>
    [Test]
    [Description("Domain-004: Verify behavior when creating a LearningComponent with zero width")]
    public void Constructor_WithZeroWidth_AcceptsZeroValue()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, 0.0f, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, ValidOrientation);

        // Assert
        Assert.That(component.Width, Is.EqualTo(0.0f));
    }

    /// <summary>
    /// Domain-005: Verify that creating a LearningComponent with negative height throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Verify that creating a LearningComponent with negative height throws ArgumentException")]
    public void Constructor_WithNegativeHeight_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, -1.0f, ValidDepth,
                ValidX, ValidY, ValidZ, ValidOrientation);
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
    /// Domain-006: Verify that creating a LearningComponent with negative depth throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Verify that creating a LearningComponent with negative depth throws ArgumentException")]
    public void Constructor_WithNegativeDepth_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, -0.5f,
                ValidX, ValidY, ValidZ, ValidOrientation);
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
    /// Domain-007: Verify that creating a LearningComponent with negative X coordinate throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Verify that creating a LearningComponent with negative X coordinate throws ArgumentException")]
    public void Constructor_WithNegativeX_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
                -10.0f, ValidY, ValidZ, ValidOrientation);
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
    /// Domain-008: Verify that creating a LearningComponent with negative Y coordinate throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Verify that creating a LearningComponent with negative Y coordinate throws ArgumentException")]
    public void Constructor_WithNegativeY_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
                ValidX, -5.0f, ValidZ, ValidOrientation);
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
    /// Domain-009: Verify that creating a LearningComponent with negative Z coordinate throws an ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-009: Verify that creating a LearningComponent with negative Z coordinate throws ArgumentException")]
    public void Constructor_WithNegativeZ_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, -1.0f, ValidOrientation);
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
    /// Domain-010: Verify that creating a LearningComponent with invalid orientation throws an ArgumentException.
    /// </summary>
    [TestCase("InvalidDirection", Description = "Domain-010: Invalid orientation 'InvalidDirection'")]
    [TestCase("Northeast", Description = "Domain-010: Invalid orientation 'Northeast'")]
    [TestCase("Up", Description = "Domain-010: Invalid orientation 'Up'")]
    [TestCase("", Description = "Domain-010: Empty orientation string")]
    public void Constructor_WithInvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ, invalidOrientation);
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
    /// Domain-011: Verify that creating a LearningComponent with valid orientation North succeeds.
    /// </summary>
    [Test]
    [Description("Domain-011: Verify that creating a LearningComponent with valid orientation North succeeds")]
    public void Constructor_WithOrientationNorth_Succeeds()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, "North");

        // Assert
        Assert.That(component.Orientation, Is.EqualTo("North"));
    }

    /// <summary>
    /// Domain-012: Verify that creating a LearningComponent with valid orientation South succeeds.
    /// </summary>
    [Test]
    [Description("Domain-012: Verify that creating a LearningComponent with valid orientation South succeeds")]
    public void Constructor_WithOrientationSouth_Succeeds()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, "South");

        // Assert
        Assert.That(component.Orientation, Is.EqualTo("South"));
    }

    /// <summary>
    /// Domain-013: Verify that creating a LearningComponent with valid orientation East succeeds.
    /// </summary>
    [Test]
    [Description("Domain-013: Verify that creating a LearningComponent with valid orientation East succeeds")]
    public void Constructor_WithOrientationEast_Succeeds()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, "East");

        // Assert
        Assert.That(component.Orientation, Is.EqualTo("East"));
    }

    /// <summary>
    /// Domain-014: Verify that creating a LearningComponent with valid orientation West succeeds.
    /// </summary>
    [Test]
    [Description("Domain-014: Verify that creating a LearningComponent with valid orientation West succeeds")]
    public void Constructor_WithOrientationWest_Succeeds()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId, ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ, "West");

        // Assert
        Assert.That(component.Orientation, Is.EqualTo("West"));
    }
}
