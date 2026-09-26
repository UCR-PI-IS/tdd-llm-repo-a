using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class BuildingMapper
{
    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
    }
}
