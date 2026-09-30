using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor validation
/// for the add-building workflow.
/// Covers intents Domain-001 through Domain-009.
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
    /// Domain-001: Verify that a Building entity is correctly instantiated
    /// with all valid properties using the add-building constructor.
    /// </summary>
    [Test]
    [Description("Domain-001: Building add constructor assigns all provided properties correctly")]
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
    /// Domain-002, Domain-003: Verify that creating a Building with a null or empty
    /// name throws ArgumentException with paramName "name".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty string name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                invalidName!, ValidColor, ValidHeight, ValidLength, ValidWidth,
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
    /// Domain-004: Verify that creating a Building with a null or empty
    /// color throws ArgumentException with paramName "color".
    /// </summary>
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty string color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? invalidColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, invalidColor!, ValidHeight, ValidLength, ValidWidth,
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
    /// throws ArgumentException with paramName "height".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float invalidHeight)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, invalidHeight, ValidLength, ValidWidth,
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
    /// throws ArgumentException with paramName "length".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(-10.0f, Description = "Domain-006: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float invalidLength)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, invalidLength, ValidWidth,
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
    /// throws ArgumentException with paramName "width".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(-8.0f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float invalidWidth)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength, invalidWidth,
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
    /// throws ArgumentException with paramName "areaId".
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_ZeroOrNegativeAreaId_ThrowsArgumentException(int invalidAreaId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
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

    /// <summary>
    /// Domain-009: Verify that a Building can be created with minimum valid positive
    /// values for dimensions (boundary values).
    /// </summary>
    [Test]
    [Description("Domain-009: Building constructor accepts minimum valid positive dimension values")]
    public void Constructor_MinimumValidDimensions_AllPropertiesSetCorrectly()
    {
        // Arrange
        var minHeight = 0.01f;
        var minLength = 0.01f;
        var minWidth = 0.01f;

        // Act
        var building = new Building(
            ValidName, ValidColor, minHeight, minLength, minWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(minHeight));
            Assert.That(building.Length, Is.EqualTo(minLength));
            Assert.That(building.Width, Is.EqualTo(minWidth));
        });
    }
}
