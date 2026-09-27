using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps building domain entities to data transfer objects.
/// </summary>
internal static class BuildingMapper
{
    /// <summary>
    /// Converts a collection of <see cref="Building"/> entities into a list of <see cref="BuildingDto"/> objects.
    /// </summary>
    /// <param name="buildings">The building entities to map.</param>
    /// <returns>A list of <see cref="BuildingDto"/> instances.</returns>
    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
    }
}
