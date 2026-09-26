using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of buildings.
/// </summary>
public record class GetBuildingListResponse(List<BuildingDto> Buildings)
{
}
