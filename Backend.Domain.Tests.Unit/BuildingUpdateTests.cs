using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity update scenarios.
/// Covers intents Domain-001 through Domain-007 for story PQL-AE-001-002.
/// </summary>
[TestFixture]
public class BuildingUpdateTests
{
    // Valid test data constants
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;

    /// <summary>
    /// Domain-001: Verify that a Building can be instantiated with all valid properties
    /// using the constructor that includes the internal identifier.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor with internalId assigns all provided properties correctly")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var building = new Building(
            ValidInternalId, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(building.InternalId, Is.EqualTo(ValidInternalId));
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
    /// Domain-002: Verify that creating a Building with an empty or null name
    /// throws <see cref="ArgumentException"/> with ParamName "name".
    /// </summary>
    [TestCase(null, Description = "Domain-002: Null name throws ArgumentException")]
    [TestCase("", Description = "Domain-002: Empty string name throws ArgumentException")]
    public void Constructor_NullOrEmptyName_ThrowsArgumentException(string? name)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(
                ValidInternalId, name!, ValidColor,
                ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("name"));
        });
    }

    /// <summary>
    /// Domain-003: Verify that creating a Building with an empty or null color
    /// throws <see cref="ArgumentException"/> with ParamName "color".
    /// </summary>
    [TestCase(null, Description = "Domain-003: Null color throws ArgumentException")]
    [TestCase("", Description = "Domain-003: Empty string color throws ArgumentException")]
    public void Constructor_NullOrEmptyColor_ThrowsArgumentException(string? color)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(
                ValidInternalId, ValidName, color!,
                ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("color"));
        });
    }

    /// <summary>
    /// Domain-004: Verify that creating a Building with zero or negative height
    /// throws <see cref="ArgumentException"/> with ParamName "height".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-004: Zero height throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-004: Negative height throws ArgumentException")]
    public void Constructor_ZeroOrNegativeHeight_ThrowsArgumentException(float height)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(
                ValidInternalId, ValidName, ValidColor,
                height, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("height"));
        });
    }

    /// <summary>
    /// Domain-005: Verify that creating a Building with zero or negative length
    /// throws <see cref="ArgumentException"/> with ParamName "length".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-005: Negative length throws ArgumentException")]
    public void Constructor_ZeroOrNegativeLength_ThrowsArgumentException(float length)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(
                ValidInternalId, ValidName, ValidColor,
                ValidHeight, length, ValidWidth,
                ValidX, ValidY, ValidZ));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("length"));
        });
    }

    /// <summary>
    /// Domain-006: Verify that creating a Building with zero or negative width
    /// throws <see cref="ArgumentException"/> with ParamName "width".
    /// </summary>
    [TestCase(0.0f, Description = "Domain-006: Zero width throws ArgumentException")]
    [TestCase(-5.0f, Description = "Domain-006: Negative width throws ArgumentException")]
    public void Constructor_ZeroOrNegativeWidth_ThrowsArgumentException(float width)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new Building(
                ValidInternalId, ValidName, ValidColor,
                ValidHeight, ValidLength, width,
                ValidX, ValidY, ValidZ));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("width"));
        });
    }

    /// <summary>
    /// Domain-007: Verify that updating an existing Building with valid new properties
    /// updates all mutable fields correctly.
    /// </summary>
    [Test]
    [Description("Domain-007: Update method correctly updates all fields of an existing building")]
    public void Update_ValidProperties_AllFieldsUpdatedCorrectly()
    {
        // Arrange
        var building = new Building(
            ValidInternalId, "Old Name", "Red",
            10.0f, 20.0f, 15.0f,
            0.0f, 0.0f, 0.0f);

        var updatedName = "Updated Building";
        var updatedColor = "Blue";
        var updatedHeight = 12.5f;
        var updatedLength = 25.0f;
        var updatedWidth = 18.0f;
        var updatedX = 100.0f;
        var updatedY = 5.0f;
        var updatedZ = 200.0f;

        // Act
        building.Update(
            updatedName, updatedColor,
            updatedHeight, updatedLength, updatedWidth,
            updatedX, updatedY, updatedZ);

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
            Assert.That(building.InternalId, Is.EqualTo(ValidInternalId));
        });
    }
}
