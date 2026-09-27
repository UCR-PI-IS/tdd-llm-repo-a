using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of buildings.
/// </summary>
public record class GetBuildingListResponse(List<BuildingDto> Buildings)
{
    public static GetBuildingListResponse FromBuildings(IEnumerable<Building> buildings)
    {
        return new GetBuildingListResponse(
            buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList());
    }
}
