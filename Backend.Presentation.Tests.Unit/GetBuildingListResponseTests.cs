using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="GetBuildingListResponse"/> record constructor.
/// Covers intent Presentation-002.
/// </summary>
[TestFixture]
public class GetBuildingListResponseTests
{
    /// <summary>
    /// Presentation-002: Verify that the GetBuildingListResponse record correctly
    /// encapsulates a list of BuildingDto objects.
    /// </summary>
    [Test]
    [Description("Presentation-002: GetBuildingListResponse encapsulates list of BuildingDto correctly")]
    public void Constructor_ValidBuildingsList_BuildingsPropertySetCorrectly()
    {
        // Arrange
        var buildings = new List<BuildingDto>
        {
            new BuildingDto(1, "Engineering Building"),
            new BuildingDto(2, "Science Building")
        };

        // Act
        var response = new GetBuildingListResponse(buildings);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.Buildings, Is.EqualTo(buildings));
            Assert.That(response.Buildings, Has.Count.EqualTo(2));
        });
    }
}
