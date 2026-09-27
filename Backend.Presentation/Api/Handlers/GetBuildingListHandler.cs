using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of buildings.
/// </summary>
public static class GetBuildingListHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch all buildings.
    /// </summary>
    /// <param name="service">Service for accessing the list of buildings.</param>
    /// <returns>An <see cref="IResult"/> containing the list of all buildings.</returns>
    public static Task<IResult> HandleAsync(IBuildingListService service)
    {
        return BuildingListResponseFactory.CreateResponseAsync(service);
    }
}
