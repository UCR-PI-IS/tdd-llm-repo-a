using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
        /// <returns>An <see cref="IResult"/> response containing the list of all buildings.</returns>
        public static async Task<IResult> HandleAsync(IBuildingListService buildingListService)
        {
            var response = await GetBuildingListResponseFactory.CreateResponseAsync(buildingListService);
            return Results.Ok(response);
        }
    }
}
