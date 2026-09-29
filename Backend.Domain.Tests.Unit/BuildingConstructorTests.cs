using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor validations.
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
    private const int ValidAreaId = 1;

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
        var areaId = ValidAreaId;

        // Act
        var building = new Building(internalId, name, color, height, length, width, x, y, z, areaId);

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
            Assert.That(building.AreaId, Is.EqualTo(areaId));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a Building with null name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Null name throws ArgumentException with paramName 'name'")]
    public void Constructor_NullName_ThrowsArgumentException()
    {
        // Arrange
        string? name = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, name!, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a Building with empty string name throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Empty string name throws ArgumentException with paramName 'name'")]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var name = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, name, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("name"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with null color throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Null color throws ArgumentException with paramName 'color'")]
    public void Constructor_NullColor_ThrowsArgumentException()
    {
        // Arrange
        string? color = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, color!, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("color"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with empty string color throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Empty string color throws ArgumentException with paramName 'color'")]
    public void Constructor_EmptyColor_ThrowsArgumentException()
    {
        // Arrange
        var color = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, color, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("color"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with zero height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Zero height throws ArgumentException with paramName 'height'")]
    public void Constructor_ZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var height = 0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Negative height throws ArgumentException with paramName 'height'")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var height = -1f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, height, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("height"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with zero length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Zero length throws ArgumentException with paramName 'length'")]
    public void Constructor_ZeroLength_ThrowsArgumentException()
    {
        // Arrange
        var length = 0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("length"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with negative length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Negative length throws ArgumentException with paramName 'length'")]
    public void Constructor_NegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var length = -1f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, length, ValidWidth, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("length"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Building with zero width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Zero width throws ArgumentException with paramName 'width'")]
    public void Constructor_ZeroWidth_ThrowsArgumentException()
    {
        // Arrange
        var width = 0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a Building with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Negative width throws ArgumentException with paramName 'width'")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var width = -1f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, width, ValidX, ValidY, ValidZ, ValidAreaId));
        Assert.That(ex.ParamName, Is.EqualTo("width"));
    }

    /// <summary>
    /// Domain-008: Verify that creating a Building with zero areaId throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Zero areaId throws ArgumentException with paramName 'areaId'")]
    public void Constructor_ZeroAreaId_ThrowsArgumentException()
    {
        // Arrange
        var areaId = 0;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, areaId));
        Assert.That(ex.ParamName, Is.EqualTo("areaId"));
    }

    /// <summary>
    /// Domain-008: Verify that creating a Building with negative areaId throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Negative areaId throws ArgumentException with paramName 'areaId'")]
    public void Constructor_NegativeAreaId_ThrowsArgumentException()
    {
        // Arrange
        var areaId = -1;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(ValidInternalId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ, areaId));
        Assert.That(ex.ParamName, Is.EqualTo("areaId"));
    }

    /// <summary>
    /// Domain-009: Verify that a Building can be created with minimum valid positive values for dimensions.
    /// </summary>
    [Test]
    [Description("Domain-009: Building can be created with minimum valid positive dimension values")]
    public void Constructor_MinimumValidDimensions_CreatesBuildingSuccessfully()
    {
        // Arrange
        var height = 0.01f;
        var length = 0.01f;
        var width = 0.01f;

        // Act
        var building = new Building(ValidInternalId, ValidName, ValidColor, height, length, width, ValidX, ValidY, ValidZ, ValidAreaId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.Height, Is.EqualTo(height));
            Assert.That(building.Length, Is.EqualTo(length));
            Assert.That(building.Width, Is.EqualTo(width));
        });
    }
}
