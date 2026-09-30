using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor with validation.
/// Covers intents Domain-001 through Domain-009 for the add-building use case.
/// Tests the constructor signature: Building(name, color, height, length, width, x, y, z, areaId).
/// </summary>
[TestFixture]
public class BuildingConstructorValidationTests
{
    // Valid test data constants
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;
    private const int ValidAreaId = 1;

    /// <summary>
    /// Domain-001, Domain-009: Verify that a Building entity is correctly instantiated
    /// with all valid properties, including boundary minimum positive dimension values.
    /// </summary>
    [TestCase(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId,
        Description = "Domain-001: Building constructor assigns all provided properties correctly")]
    [TestCase("Small Shed", "Blue", 0.1f, 0.1f, 0.1f, -50.0f, -50.0f, -10.0f, 1,
        Description = "Domain-009: Building constructor accepts minimum valid positive dimensions and negative coordinates")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly(
        string name, string color, float height, float length, float width,
        float x, float y, float z, int areaId)
    {
        // Arrange & Act
        var building = new Building(name, color, height, length, width, x, y, z, areaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Name, Is.EqualTo(name));
            Assert.That(building.Color, Is.EqualTo(color));
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
            Assert.That(building.X, Is.EqualTo(x));
            Assert.That(building.Y, Is.EqualTo(y));
            Assert.That(building.Z, Is.EqualTo(z));
            Assert.That(building.AreaId, Is.EqualTo(areaId));
        });
    }

    /// <summary>
    /// Domain-002, Domain-003: Verify that creating a Building with a null or empty name
    /// throws ArgumentException with ParamName "name".
    /// </summary>
    [TestCase(null!, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty string name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string invalidName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(invalidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
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
    /// Domain-004: Verify that creating a Building with a null or empty color
    /// throws ArgumentException with ParamName "color".
    /// </summary>
    [TestCase(null!, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty string color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string invalidColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, invalidColor, ValidHeight, ValidLength, ValidWidth,
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
    /// Domain-005: Verify that creating a Building with zero or negative height
    /// throws ArgumentException with ParamName "height".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float invalidHeight)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, ValidColor, invalidHeight, ValidLength, ValidWidth,
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
    /// Domain-006: Verify that creating a Building with zero or negative length
    /// throws ArgumentException with ParamName "length".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(-10.0f, Description = "Domain-006: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float invalidLength)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, ValidColor, ValidHeight, invalidLength, ValidWidth,
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
    /// Domain-007: Verify that creating a Building with zero or negative width
    /// throws ArgumentException with ParamName "width".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(-8.0f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float invalidWidth)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, ValidColor, ValidHeight, ValidLength, invalidWidth,
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
    /// Domain-008: Verify that creating a Building with zero or negative areaId
    /// throws ArgumentException with ParamName "areaId".
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_ZeroOrNegativeAreaId_ThrowsArgumentException(int invalidAreaId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, invalidAreaId);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("areaId"));
        });
    }
}
