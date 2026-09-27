using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object containing the list of buildings.
/// </summary>
/// <param name="Buildings">The list of building DTOs.</param>
public record class GetBuildingListResponse(
    List<BuildingDto> Buildings);
