using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor.
/// Covers intents Domain-001 through Domain-009 for story PQL-AE-001-001.
/// </summary>
[TestFixture]
public class BuildingTests
{
    /// <summary>
    /// Domain-001 (original): Verify that a Building entity is correctly instantiated with all required properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor assigns all provided properties correctly")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var internalId = 1;
        var name = "Engineering Building";
        var color = "Red";
        var height = 20.5f;
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;

        // Act
        var building = new Building(internalId, name, color, height, length, width, x, y, z);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.InternalId, Is.EqualTo(internalId));
            Assert.That(building.Name, Is.EqualTo(name));
            Assert.That(building.Color, Is.EqualTo(color));
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
            Assert.That(building.X, Is.EqualTo(x));
            Assert.That(building.Y, Is.EqualTo(y));
            Assert.That(building.Z, Is.EqualTo(z));
        });
    }

    /// <summary>
    /// Domain-001 (PQL-AE-001-001): Verify that a Building entity with areaId is correctly instantiated with all valid properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor with areaId assigns all properties correctly")]
    public void Constructor_WithAreaId_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var name = "Engineering Building";
        var color = "Red";
        var height = 20.5f;
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
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
    /// Domain-002, Domain-003: Verify that creating a Building with null or empty name throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty name throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange
        var color = "Red";
        var height = 20.5f;
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(invalidName!, color, height, length, width, x, y, z, areaId);
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
    /// Domain-004: Verify that creating a Building with null or empty color throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty color throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidColor_ThrowsArgumentException(string? invalidColor)
    {
        // Arrange
        var name = "Engineering Building";
        var height = 20.5f;
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(name, invalidColor!, height, length, width, x, y, z, areaId);
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
    /// Domain-005: Verify that creating a Building with zero or negative height throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-005: Negative height throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidHeight_ThrowsArgumentException(float invalidHeight)
    {
        // Arrange
        var name = "Engineering Building";
        var color = "Red";
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(name, color, invalidHeight, length, width, x, y, z, areaId);
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
    /// Domain-006: Verify that creating a Building with zero or negative length throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-006: Negative length throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidLength_ThrowsArgumentException(float invalidLength)
    {
        // Arrange
        var name = "Engineering Building";
        var color = "Red";
        var height = 20.5f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(name, color, height, invalidLength, width, x, y, z, areaId);
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
    /// Domain-007: Verify that creating a Building with zero or negative width throws ArgumentException.
    /// </summary>
    [TestCase(0f, Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidWidth_ThrowsArgumentException(float invalidWidth)
    {
        // Arrange
        var name = "Engineering Building";
        var color = "Red";
        var height = 20.5f;
        var length = 50.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(name, color, height, length, invalidWidth, x, y, z, areaId);
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
    /// Domain-008: Verify that creating a Building with zero or negative areaId throws ArgumentException.
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_WithAreaId_InvalidAreaId_ThrowsArgumentException(int invalidAreaId)
    {
        // Arrange
        var name = "Engineering Building";
        var color = "Red";
        var height = 20.5f;
        var length = 50.0f;
        var width = 30.0f;
        var x = 100.0f;
        var y = 200.0f;
        var z = 0.0f;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(name, color, height, length, width, x, y, z, invalidAreaId);
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

    /// <summary>
    /// Domain-009: Verify that a Building can be created with minimum valid positive values for dimensions.
    /// </summary>
    [Test]
    [Description("Domain-009: Minimum valid positive dimensions create building correctly")]
    public void Constructor_WithAreaId_MinimumValidDimensions_PropertiesSetCorrectly()
    {
        // Arrange
        var name = "Small Building";
        var color = "Blue";
        var height = 0.1f;
        var length = 0.1f;
        var width = 0.1f;
        var x = 0.0f;
        var y = 0.0f;
        var z = 0.0f;
        var areaId = 1;

        // Act
        var building = new Building(name, color, height, length, width, x, y, z, areaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
        });
    }
}
