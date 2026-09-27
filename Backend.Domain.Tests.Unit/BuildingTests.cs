using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor.
/// Covers intent Domain-001.
/// </summary>
[TestFixture]
public class BuildingTests
{
    /// <summary>
    /// Domain-001: Verify that a Building entity is correctly instantiated with all required properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building constructor sets all properties correctly")]
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
}
