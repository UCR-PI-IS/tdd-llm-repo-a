using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="BuildingDto"/> record.
/// Covers intent Presentation-001.
/// </summary>
[TestFixture]
public class BuildingDtoTests
{
    // Valid test data
    private const int ValidId = 1;
    private const string ValidName = "Engineering Building";

    /// <summary>
    /// Presentation-001: Verify BuildingDto record is correctly instantiated with Id and Name properties.
    /// </summary>
    [Test]
    [Description("Presentation-001: BuildingDto constructor sets Id and Name correctly")]
    public void Constructor_ValidIdAndName_AllPropertiesSetCorrectly()
    {
        // Arrange & Act
        var dto = new BuildingDto(ValidId, ValidName);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dto.Id, Is.EqualTo(ValidId));
            Assert.That(dto.Name, Is.EqualTo(ValidName));
        });
    }
}
