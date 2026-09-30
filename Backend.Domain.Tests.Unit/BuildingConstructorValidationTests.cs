using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor validation.
/// Covers intents Domain-001 through Domain-009 for story PQL-AE-001-001.
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
    /// Domain-001: Verify that a Building entity is correctly instantiated with all valid properties.
    /// Domain-009: Verify that a Building can be created with minimum valid positive values for dimensions.
    /// </summary>
    [TestCase(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId,
        Description = "Domain-001: Valid Building with all parameters correctly assigned")]
    [TestCase(ValidName, ValidColor, 0.001f, 0.001f, 0.001f, ValidX, ValidY, ValidZ, ValidAreaId,
        Description = "Domain-009: Valid Building with minimum positive dimension boundary values")]
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
    /// Domain-002 and Domain-003: Verify that creating a Building with a null or empty name
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? invalidName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(invalidName!, ValidColor, ValidHeight, ValidLength, ValidWidth,
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
    /// throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? invalidColor)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, invalidColor!, ValidHeight, ValidLength, ValidWidth,
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
    /// Domain-005, Domain-006, Domain-007: Verify that creating a Building with zero or negative
    /// dimensions (height, length, width) throws ArgumentException with the correct parameter name.
    /// </summary>
    [TestCase(0.0f, ValidLength, ValidWidth, "height",
        Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-5.0f, ValidLength, ValidWidth, "height",
        Description = "Domain-005: Negative height throws ArgumentException")]
    [TestCase(ValidHeight, 0.0f, ValidWidth, "length",
        Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(ValidHeight, -10.0f, ValidWidth, "length",
        Description = "Domain-006: Negative length throws ArgumentException")]
    [TestCase(ValidHeight, ValidLength, 0.0f, "width",
        Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(ValidHeight, ValidLength, -8.0f, "width",
        Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeDimension_ThrowsArgumentException(
        float height, float length, float width, string expectedParamName)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new Building(ValidName, ValidColor, height, length, width,
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
    /// Domain-008: Verify that creating a Building with zero or negative areaId
    /// throws ArgumentException with the correct parameter name.
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
