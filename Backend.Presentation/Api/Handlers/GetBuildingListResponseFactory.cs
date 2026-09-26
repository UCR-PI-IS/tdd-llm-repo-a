using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for building the response payload for the get building list operation.
/// </summary>
internal static class GetBuildingListResponseFactory
{
    /// <summary>
    /// Creates the response payload containing the list of all buildings.
    /// </summary>
    public static async Task<GetBuildingListResponse> CreateResponseAsync(IBuildingListService buildingListService)
    {
        var buildings = await buildingListService.GetAllBuildingsAsync();
        var dtos = BuildingMapper.ToDtoList(buildings);
        return new GetBuildingListResponse(dtos);
    }
}
