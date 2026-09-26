using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingDto"/>.
/// Covers intent Presentation-001.
/// </summary>
[TestFixture]
public class BuildingDtoTests
{
    /// <summary>
    /// Presentation-001: Verify BuildingDto record is correctly instantiated with Id and Name properties.
    /// </summary>
    [Test]
    [Description("Presentation-001: BuildingDto record sets Id and Name properties correctly")]
    public void Constructor_ValidParameters_PropertiesSetCorrectly()
    {
        // Arrange
        var id = 1;
        var name = "Engineering Building";

        // Act
        var dto = new BuildingDto(id, name);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(id));
            Assert.That(dto.Name, Is.EqualTo(name));
        });
    }
}
