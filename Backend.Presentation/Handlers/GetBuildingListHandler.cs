using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers
{
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
        public static async Task<Ok<GetBuildingListResponse>> HandleAsync(IBuildingListService buildingListService)
        {
            var buildings = await buildingListService.GetAllBuildingsAsync();
            return BuildingListHelper.BuildResponse(buildings);
        }
    }
}
