using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of buildings.
/// </summary>
public static class GetBuildingListHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch all buildings.
    /// </summary>
    /// <param name="buildingListService">Service for accessing the list of buildings.</param>
    /// <returns>An <see cref="Ok{T}"/> response containing the list of all buildings.</returns>
    public static async Task<Ok<GetBuildingListResponse>> HandleAsync([FromServices] IBuildingListService buildingListService)
    {
        var buildings = await buildingListService.GetAllBuildingsAsync();
        var dtos = buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
        var response = new GetBuildingListResponse(dtos);
        return TypedResults.Ok(response);
    }
}
