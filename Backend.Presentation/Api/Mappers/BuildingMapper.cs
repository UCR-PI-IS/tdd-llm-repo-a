using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Mapper for converting Building entities to BuildingDto objects.
/// </summary>
internal static class BuildingMapper
{
    /// <summary>
    /// Converts a collection of Building entities to a list of BuildingDto objects.
    /// </summary>
    /// <param name="buildings">The collection of Building entities to convert.</param>
    /// <returns>A list of BuildingDto objects.</returns>
    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(building => new BuildingDto(
            building.InternalId,
            building.Name)).ToList();
    }
}
