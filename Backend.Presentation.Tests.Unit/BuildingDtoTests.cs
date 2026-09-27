using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="BuildingDto"/> record constructor.
/// Covers intent Presentation-001.
/// </summary>
[TestFixture]
public class BuildingDtoTests
{
    /// <summary>
    /// Presentation-001: Verify BuildingDto record is correctly instantiated
    /// with Id and Name properties.
    /// </summary>
    [Test]
    [Description("Presentation-001: Verify BuildingDto record is correctly instantiated with Id and Name properties")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
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
