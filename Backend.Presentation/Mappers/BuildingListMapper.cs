using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps building domain entities to presentation DTOs and responses.
/// </summary>
public static class BuildingListMapper
{
    /// <summary>
    /// Converts a list of building entities into a <see cref="GetBuildingListResponse"/>.
    /// </summary>
    /// <param name="buildings">The list of building entities.</param>
    /// <returns>A response containing the mapped building DTOs.</returns>
    public static GetBuildingListResponse ToResponse(List<Building> buildings)
    {
        var dtos = buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
        return new GetBuildingListResponse(dtos);
    }
}
