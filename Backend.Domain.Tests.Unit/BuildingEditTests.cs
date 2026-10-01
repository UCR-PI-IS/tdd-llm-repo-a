using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity edit/update operations.
/// Covers intents Domain-001 through Domain-007 for story PQL-AE-001-002 (Edit Building).
/// </summary>
[TestFixture]
public class BuildingEditTests
{
    // Valid test data constants
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;

    /// <summary>
    /// Domain-001: Verify that a Building can be created with all valid properties
    /// using the constructor with internalId.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor with internalId assigns all provided properties correctly")]
    public void Constructor_WithInternalIdValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var building = new Building(
            ValidInternalId, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.InternalId, Is.EqualTo(ValidInternalId));
            Assert.That(building.Name, Is.EqualTo(ValidName));
            Assert.That(building.Color, Is.EqualTo(ValidColor));
            Assert.That(building.Height, Is.EqualTo(ValidHeight));
            Assert.That(building.Length, Is.EqualTo(ValidLength));
            Assert.That(building.Width, Is.EqualTo(ValidWidth));
            Assert.That(building.X, Is.EqualTo(ValidX));
            Assert.That(building.Y, Is.EqualTo(ValidY));
            Assert.That(building.Z, Is.EqualTo(ValidZ));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a Building with a null or empty name
    /// throws ArgumentException with ParamName "name".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-002: Empty name throws ArgumentException")]
    public void Constructor_WithInternalIdNullOrEmptyName_ThrowsArgumentException(string? name)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidInternalId, name!, ValidColor,
                ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("name"));
        });
    }

    /// <summary>
    /// Domain-003: Verify that creating a Building with a null or empty color
    /// throws ArgumentException with ParamName "color".
    /// </summary>
    [TestCase(null, Description = "Domain-003: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty color throws ArgumentException")]
    public void Constructor_WithInternalIdNullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidInternalId, ValidName, color!,
                ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("color"));
        });
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with zero or negative height
    /// throws ArgumentException with ParamName "height".
    /// </summary>
    [TestCase(0f, Description = "Domain-004: Zero height throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-004: Negative height throws ArgumentException")]
    public void Constructor_WithInternalIdZeroOrNegativeHeight_ThrowsArgumentException(float height)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidInternalId, ValidName, ValidColor,
                height, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ);
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
    /// Domain-005: Verify that creating a Building with zero or negative length
    /// throws ArgumentException with ParamName "length".
    /// </summary>
    [TestCase(0f, Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative length throws ArgumentException")]
    public void Constructor_WithInternalIdZeroOrNegativeLength_ThrowsArgumentException(float length)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidInternalId, ValidName, ValidColor,
                ValidHeight, length, ValidWidth,
                ValidX, ValidY, ValidZ);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("length"));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with zero or negative width
    /// throws ArgumentException with ParamName "width".
    /// </summary>
    [TestCase(0f, Description = "Domain-006: Zero width throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-006: Negative width throws ArgumentException")]
    public void Constructor_WithInternalIdZeroOrNegativeWidth_ThrowsArgumentException(float width)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidInternalId, ValidName, ValidColor,
                ValidHeight, ValidLength, width,
                ValidX, ValidY, ValidZ);
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
    /// Domain-007: Verify that updating an existing Building with valid new properties
    /// updates all fields correctly.
    /// </summary>
    [Test]
    [Description("Domain-007: Update method updates all building fields correctly with valid parameters")]
    public void Update_ValidParameters_AllFieldsUpdatedCorrectly()
    {
        // Arrange
        var building = new Building(1, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0f, 0f, 0f);
        var newName = "Updated Building";
        var newColor = "Blue";
        var newHeight = 12.5f;
        var newLength = 25.0f;
        var newWidth = 18.0f;
        var newX = 100.0f;
        var newY = 5.0f;
        var newZ = 200.0f;

        // Act
        building.Update(newName, newColor, newHeight, newLength, newWidth, newX, newY, newZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Name, Is.EqualTo(newName));
            Assert.That(building.Color, Is.EqualTo(newColor));
            Assert.That(building.Height, Is.EqualTo(newHeight));
            Assert.That(building.Length, Is.EqualTo(newLength));
            Assert.That(building.Width, Is.EqualTo(newWidth));
            Assert.That(building.X, Is.EqualTo(newX));
            Assert.That(building.Y, Is.EqualTo(newY));
            Assert.That(building.Z, Is.EqualTo(newZ));
        });
    }
}
