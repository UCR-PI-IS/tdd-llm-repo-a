using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Building"/> entity constructor and properties.
/// Covers intents Domain-001 and Domain-002.
/// </summary>
[TestFixture]
public class BuildingTests
{
    // Valid test data constants
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
    /// Domain-001: Verify that a Building entity is correctly instantiated with all required properties.
    /// </summary>
    [Test]
    [Description("Domain-001: Building entity is correctly instantiated with all required properties")]
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
    /// Domain-002: Verify repository interface defines GetAllBuildingsAsync method returning Task&lt;List&lt;Building&gt;&gt;.
    /// </summary>
    [Test]
    [Description("Domain-002: IBuildingListRepository interface defines GetAllBuildingsAsync method")]
    public void IBuildingListRepository_InterfaceContract_DefinesGetAllBuildingsAsync()
    {
        // Arrange & Act
        var method = typeof(IBuildingListRepository).GetMethod("GetAllBuildingsAsync");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(method, Is.Not.Null);
            Assert.That(method!.ReturnType, Is.EqualTo(typeof(Task<List<Building>>)));
            Assert.That(method.GetParameters(), Is.Empty);
        });
    }
}
