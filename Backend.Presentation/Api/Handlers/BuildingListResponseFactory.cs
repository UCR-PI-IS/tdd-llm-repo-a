using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses for the building list operation.
/// </summary>
internal static class BuildingListResponseFactory
{
    /// <summary>
    /// Creates an OK response containing the list of all buildings.
    /// </summary>
    /// <param name="service">Service for accessing building data.</param>
    /// <returns>A task containing an <see cref="IResult"/> with the building list.</returns>
    public static async Task<IResult> CreateResponseAsync(IBuildingListService service)
    {
        return TypedResults.Ok(new GetBuildingListResponse(BuildingMapper.ToDtoList(await service.GetAllBuildingsAsync())));
    }
}
