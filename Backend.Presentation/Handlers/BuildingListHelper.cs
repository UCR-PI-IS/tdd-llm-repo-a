using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Helper for building the HTTP result for the building list endpoint.
/// Encapsulates domain-to-DTO mapping and response construction to keep
/// <see cref="GetBuildingListHandler"/> simple and reduce class coupling.
/// </summary>
internal static class BuildingListHelper
{
    /// <summary>
    /// Maps a list of <see cref="Building"/> entities into an
    /// <see cref="Ok{GetBuildingListResponse}"/> result.
    /// </summary>
    /// <param name="buildings">The domain entities returned by the service.</param>
    /// <returns>An <c>OK</c> result containing the building list response.</returns>
    public static Ok<GetBuildingListResponse> BuildResponse(List<Building> buildings)
    {
        var dtos = new List<BuildingDto>(buildings.Count);
        for (int i = 0; i < buildings.Count; i++)
        {
            dtos.Add(new BuildingDto(buildings[i].InternalId, buildings[i].Name));
        }
        return TypedResults.Ok(new GetBuildingListResponse(dtos));
    }
}
