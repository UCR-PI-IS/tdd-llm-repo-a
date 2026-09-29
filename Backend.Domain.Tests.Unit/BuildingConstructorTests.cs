using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-009.
/// </summary>
[TestFixture]
public class BuildingConstructorTests
{
    // Valid test data constants
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 15.0f;
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
    [Description("Domain-001: Building constructor with all valid properties sets values correctly")]
    public void Constructor_ValidProperties_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var building = new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId);

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
    /// Domain-002: Verify that creating a Building with null name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Null name throws ArgumentException")]
    public void Constructor_NullName_ThrowsArgumentException()
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(null!, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a Building with empty string name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Empty string name throws ArgumentException")]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(string.Empty, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with null or empty color throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-004: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-004: Empty color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidName, color!, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("color"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with zero or negative height throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-005: Zero height throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-005: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float height)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with zero or negative length throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-006: Zero length throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-006: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float length)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("length"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Building with zero or negative width throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-007: Zero width throws ArgumentException")]
    [TestCase(-1.0f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float width)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ, ValidAreaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Domain-008: Verify that creating a Building with zero or negative areaId throws ArgumentException.
    /// </summary>
    [TestCase(0, Description = "Domain-008: Zero areaId throws ArgumentException")]
    [TestCase(-1, Description = "Domain-008: Negative areaId throws ArgumentException")]
    public void Constructor_ZeroOrNegativeAreaId_ThrowsArgumentException(int areaId)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, areaId));

        // Assert
        Assert.That(ex.ParamName, Is.EqualTo("areaId"));
    }

    /// <summary>
    /// Domain-009: Verify that a Building can be created with minimum valid positive values for dimensions.
    /// </summary>
    [Test]
    [Description("Domain-009: Building can be created with minimum valid positive boundary values")]
    public void Constructor_MinimumValidDimensions_PropertiesSetCorrectly()
    {
        // Arrange
        const float minHeight = 0.1f;
        const float minLength = 0.1f;
        const float minWidth = 0.1f;

        // Act
        var building = new Building(ValidName, ValidColor, minHeight, minLength, minWidth, ValidX, ValidY, ValidZ, ValidAreaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(minHeight));
            Assert.That(building.Length, Is.EqualTo(minLength));
            Assert.That(building.Width, Is.EqualTo(minWidth));
        });
    }
}
