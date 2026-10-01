using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity update operations and constructor validation.
/// Covers intents Domain-001 through Domain-007 for PQL-AE-001-002.
/// </summary>
[TestFixture]
public class BuildingUpdateTests
{
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;

    /// <summary>
    /// Domain-001: Verify that a Building can be created with all valid properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor with valid parameters sets all properties correctly")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var internalId = 1;

        // Act
        var building = new Building(
            internalId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.InternalId, Is.EqualTo(internalId));
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
    /// Domain-002: Verify that creating a Building with null or empty name throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-002: Empty name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? name)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(1, name!, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);
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
    /// Domain-003: Verify that creating a Building with null or empty color throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-003: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(1, ValidName, color!, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);
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
    /// Domain-004: Verify that creating a Building with zero or negative height throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-004: Zero height throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-004: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float height)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(1, ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);
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
    [TestCase(0.0f, Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float length)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(1, ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ);
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
    [TestCase(0.0f, Description = "Domain-006: Zero width throws ArgumentException")]
    [TestCase(-15.0f, Description = "Domain-006: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float width)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(1, ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ);
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
    /// </summary>
    [Test]
    [Description("Domain-007: Update method modifies all properties of an existing building correctly")]
    public void Update_ValidParameters_UpdatesAllPropertiesCorrectly()
    {
        // Arrange
        var building = new Building(1, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0.0f, 0.0f, 0.0f);
        var updatedName = "Updated Building";
        var updatedColor = "Blue";
        var updatedHeight = 12.5f;
        var updatedLength = 25.0f;
        var updatedWidth = 18.0f;
        var updatedX = 100.0f;
        var updatedY = 5.0f;
        var updatedZ = 200.0f;

        // Act
        building.Update(updatedName, updatedColor, updatedHeight, updatedLength, updatedWidth, updatedX, updatedY, updatedZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Name, Is.EqualTo(updatedName));
            Assert.That(building.Color, Is.EqualTo(updatedColor));
            Assert.That(building.Height, Is.EqualTo(updatedHeight));
            Assert.That(building.Length, Is.EqualTo(updatedLength));
            Assert.That(building.Width, Is.EqualTo(updatedWidth));
            Assert.That(building.X, Is.EqualTo(updatedX));
            Assert.That(building.Y, Is.EqualTo(updatedY));
            Assert.That(building.Z, Is.EqualTo(updatedZ));
        });
    }
}
