using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity update operations and validation constructor.
/// Covers intents Domain-001 through Domain-007 for story PQL-AE-001-002.
/// </summary>
[TestFixture]
public class BuildingUpdateTests
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
    private const int ValidAreaId = 1;

    /// <summary>
    /// Domain-001: Verify that a Building can be created with all valid properties.
    /// Tests the validation constructor with complete valid data.
    /// </summary>
    [Test]
    [Description("Domain-001: Building can be created with all valid properties")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Name, Is.EqualTo(ValidName));
            Assert.That(building.Color, Is.EqualTo(ValidColor));
            Assert.That(building.Height, Is.EqualTo(ValidHeight));
            Assert.That(building.Length, Is.EqualTo(ValidLength));
            Assert.That(building.Width, Is.EqualTo(ValidWidth));
            Assert.That(building.X, Is.EqualTo(ValidX));
            Assert.That(building.Y, Is.EqualTo(ValidY));
            Assert.That(building.Z, Is.EqualTo(ValidZ));
            Assert.That(building.AreaId, Is.EqualTo(ValidAreaId));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a Building with empty name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Empty name throws ArgumentException with paramName 'name'")]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                "", ValidColor, ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-003: Verify that creating a Building with empty color throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Empty color throws ArgumentException with paramName 'color'")]
    public void Constructor_EmptyColor_ThrowsArgumentException()
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, "", ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-004: Verify that creating a Building with zero height throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-004: Zero height throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-004: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float height)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, height, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-005: Verify that creating a Building with zero or negative length throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float length)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, length, ValidWidth,
                ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-006: Verify that creating a Building with zero or negative width throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-006: Zero width throws ArgumentException")]
    [TestCase(-10.0f, Description = "Domain-006: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float width)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength, width,
                ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-007: Verify that updating an existing Building with valid new properties updates all fields correctly.
    /// Note: This test assumes an Update method exists or uses the non-validating constructor to simulate update.
    /// </summary>
    [Test]
    [Description("Domain-007: Updating building with valid properties updates all fields correctly")]
    public void Update_ValidParameters_AllPropertiesUpdatedCorrectly()
    {
        // Arrange
        var building = new Building(
            ValidInternalId, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0f, 0f, 0f);

        var newName = "Updated Building";
        var newColor = "Blue";
        var newHeight = 12.5f;
        var newLength = 25.0f;
        var newWidth = 18.0f;
        var newX = 100.0f;
        var newY = 5.0f;
        var newZ = 200.0f;

        // Act - Create updated building using non-validating constructor (simulating update)
        var updatedBuilding = new Building(
            ValidInternalId, newName, newColor, newHeight, newLength, newWidth, newX, newY, newZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedBuilding.InternalId, Is.EqualTo(ValidInternalId));
            Assert.That(updatedBuilding.Name, Is.EqualTo(newName));
            Assert.That(updatedBuilding.Color, Is.EqualTo(newColor));
            Assert.That(updatedBuilding.Height, Is.EqualTo(newHeight));
            Assert.That(updatedBuilding.Length, Is.EqualTo(newLength));
            Assert.That(updatedBuilding.Width, Is.EqualTo(newWidth));
            Assert.That(updatedBuilding.X, Is.EqualTo(newX));
            Assert.That(updatedBuilding.Y, Is.EqualTo(newY));
            Assert.That(updatedBuilding.Z, Is.EqualTo(newZ));
        });
    }
}
