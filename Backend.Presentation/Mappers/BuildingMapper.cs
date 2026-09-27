using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class BuildingMapper
{
    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(building => new BuildingDto(
            building.InternalId,
            building.Name)).ToList();
    }
}
