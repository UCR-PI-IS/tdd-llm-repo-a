using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor.
/// Covers intents Domain-001 through Domain-009.
/// </summary>
[TestFixture]
public class BuildingConstructorTests
{
    // Valid test data
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;

    /// <summary>
    /// Domain-001: Verify that a Building entity is correctly instantiated with all valid properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor assigns all provided properties correctly")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var internalId = ValidInternalId;
        var name = ValidName;
        var color = ValidColor;
        var height = ValidHeight;
        var length = ValidLength;
        var width = ValidWidth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;

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
    /// Domain-002: Verify that creating a Building with null name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Building constructor throws ArgumentException when name is null")]
    public void Constructor_NullName_ThrowsArgumentException()
    {
        // Arrange
        string? name = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, name!, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a Building with empty string name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Building constructor throws ArgumentException when name is empty string")]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var name = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, name, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with null color throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Building constructor throws ArgumentException when color is null")]
    public void Constructor_NullColor_ThrowsArgumentException()
    {
        // Arrange
        string? color = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, color!, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("color"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with empty string color throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Building constructor throws ArgumentException when color is empty string")]
    public void Constructor_EmptyColor_ThrowsArgumentException()
    {
        // Arrange
        var color = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, color, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("color"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with zero height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Building constructor throws ArgumentException when height is zero")]
    public void Constructor_ZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var height = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Building constructor throws ArgumentException when height is negative")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var height = -10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with zero length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Building constructor throws ArgumentException when length is zero")]
    public void Constructor_ZeroLength_ThrowsArgumentException()
    {
        // Arrange
        var length = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("length"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with negative length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Building constructor throws ArgumentException when length is negative")]
    public void Constructor_NegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var length = -20.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("length"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Building with zero width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Building constructor throws ArgumentException when width is zero")]
    public void Constructor_ZeroWidth_ThrowsArgumentException()
    {
        // Arrange
        var width = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Building with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Building constructor throws ArgumentException when width is negative")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var width = -15.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Domain-009: Verify that a Building can be created with minimum valid positive values for dimensions.
    /// </summary>
    [Test]
    [Description("Domain-009: Building constructor accepts minimum valid positive dimension values")]
    public void Constructor_MinimumValidDimensions_CreatesBuildingSuccessfully()
    {
        // Arrange
        var height = 0.1f;
        var length = 0.1f;
        var width = 0.1f;

        // Act
        var building = new Building(ValidInternalId, ValidName, ValidColor, height, length, width, ValidX, ValidY, ValidZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
        });
    }
}
