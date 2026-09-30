using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor validation.
/// Covers intents Domain-001 through Domain-009 for the Add Building story.
/// </summary>
[TestFixture]
public class BuildingValidationTests
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
    [Description("Domain-001: Building constructor assigns all provided properties correctly with valid input")]
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
    /// Domain-002 and Domain-003: Verify that creating a Building with a null or empty name
    /// throws ArgumentException with ParamName "name".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty string name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? name)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                name!, ValidColor, ValidHeight, ValidLength, ValidWidth,
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
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty string color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, color!, ValidHeight, ValidLength, ValidWidth,
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
    /// Domain-005, Domain-006, Domain-007: Verify that creating a Building with a zero or negative
    /// dimension (height, length, or width) throws ArgumentException with the appropriate ParamName.
    /// </summary>
    [TestCase(0.0f, ValidLength, ValidWidth, "height",
        Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-1.0f, ValidLength, ValidWidth, "height",
        Description = "Domain-005: Negative height throws ArgumentException")]
    [TestCase(ValidHeight, 0.0f, ValidWidth, "length",
        Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(ValidHeight, -50.0f, ValidWidth, "length",
        Description = "Domain-006: Negative length throws ArgumentException")]
    [TestCase(ValidHeight, ValidLength, 0.0f, "width",
        Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(ValidHeight, ValidLength, -30.0f, "width",
        Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeDimension_ThrowsArgumentException(
        float height, float length, float width, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, height, length, width,
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
            Assert.That(caughtException!.ParamName, Is.EqualTo(expectedParamName));
        });
    }

    /// <summary>
    /// Domain-008: Verify that creating a Building with a zero or negative areaId
    /// throws ArgumentException with ParamName "areaId".
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_ZeroOrNegativeAreaId_ThrowsArgumentException(int areaId)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(
                ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, areaId);
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
    /// Domain-009: Verify that a Building can be created with minimum valid positive values
    /// for dimensions (boundary values at the smallest positive float).
    /// </summary>
    [Test]
    [Description("Domain-009: Building can be created with minimum valid positive dimension values")]
    public void Constructor_MinimumValidDimensions_PropertiesSetCorrectly()
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
