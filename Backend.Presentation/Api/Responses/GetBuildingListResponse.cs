using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of buildings.
/// </summary>
public record class GetBuildingListResponse(List<BuildingDto> Buildings)
{
    /// <summary>
    /// Creates an OK response containing the list of building DTOs from domain entities.
    /// </summary>
    /// <param name="buildings">The domain entities to include in the response.</param>
    /// <returns>An <see cref="Ok{T}"/> result containing the response.</returns>
    public static Ok<GetBuildingListResponse> CreateOk(List<Building> buildings)
    {
        var dtos = new List<BuildingDto>(buildings.Count);
        for (int i = 0; i < buildings.Count; i++)
        {
            dtos.Add(new BuildingDto(buildings[i].InternalId, buildings[i].Name));
        }

        return TypedResults.Ok(new GetBuildingListResponse(dtos));
    }
}
