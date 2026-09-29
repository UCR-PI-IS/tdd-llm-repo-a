using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor when adding a new building.
/// Covers intents Domain-001 through Domain-009 for PQL-AE-001-001.
/// </summary>
[TestFixture]
public class BuildingConstructorTests
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
    /// Domain-001: Verify that a Building entity is correctly instantiated with all valid properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor assigns all provided properties correctly")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength,
            ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);

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
    /// Domain-002 & Domain-003: Verify that creating a Building with null or empty name
    /// throws ArgumentException with parameter name "name".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty name throws ArgumentException")]
    public void Constructor_InvalidName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                invalidName!, ValidColor, ValidHeight, ValidLength,
                ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// Domain-004: Verify that creating a Building with null or empty color
    /// throws ArgumentException with parameter name "color".
    /// </summary>
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty color throws ArgumentException")]
    public void Constructor_InvalidColor_ThrowsArgumentException(string? invalidColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, invalidColor!, ValidHeight, ValidLength,
                ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// throws ArgumentException with parameter name "height".
    /// </summary>
    [TestCase(0f, Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-005: Negative height throws ArgumentException")]
    public void Constructor_InvalidHeight_ThrowsArgumentException(float invalidHeight)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, invalidHeight, ValidLength,
                ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// throws ArgumentException with parameter name "length".
    /// </summary>
    [TestCase(0f, Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-006: Negative length throws ArgumentException")]
    public void Constructor_InvalidLength_ThrowsArgumentException(float invalidLength)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, invalidLength,
                ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// throws ArgumentException with parameter name "width".
    /// </summary>
    [TestCase(0f, Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(-1f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_InvalidWidth_ThrowsArgumentException(float invalidWidth)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength,
                invalidWidth, ValidX, ValidY, ValidZ, ValidAreaId);
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
    /// throws ArgumentException with parameter name "areaId".
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_InvalidAreaId_ThrowsArgumentException(int invalidAreaId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength,
                ValidWidth, ValidX, ValidY, ValidZ, invalidAreaId);
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
    [Description("Domain-009: Building can be created with minimum valid positive dimensions")]
    public void Constructor_MinimumValidDimensions_AllDimensionPropertiesSetCorrectly()
    {
        // Arrange
        var minHeight = 0.1f;
        var minLength = 0.1f;
        var minWidth = 0.1f;
        var minAreaId = 1;

        // Act
        var building = new Building(
            ValidName, ValidColor, minHeight, minLength,
            minWidth, ValidX, ValidY, ValidZ, minAreaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(minHeight));
            Assert.That(building.Length, Is.EqualTo(minLength));
            Assert.That(building.Width, Is.EqualTo(minWidth));
            Assert.That(building.AreaId, Is.EqualTo(minAreaId));
        });
    }
}
