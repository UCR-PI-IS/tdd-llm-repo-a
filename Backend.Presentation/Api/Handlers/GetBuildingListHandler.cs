using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for retrieving the list of all buildings.
/// </summary>
public static class GetBuildingListHandler
{
    /// <summary>
    /// Handles the asynchronous request to retrieve all buildings.
    /// </summary>
    /// <param name="service">The building list service.</param>
    /// <returns>An <see cref="Ok{GetBuildingListResponse}"/> containing the list of buildings.</returns>
    public static async Task<Ok<GetBuildingListResponse>> HandleAsync(IBuildingListService service)
    {
        var buildings = await service.GetAllBuildingsAsync();
        var buildingDtos = BuildingMapper.ToDtoList(buildings);
        return TypedResults.Ok(new GetBuildingListResponse(buildingDtos));
    }
}
