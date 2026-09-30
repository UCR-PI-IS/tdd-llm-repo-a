using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class BuildingMapper
{
    public static Building ToEntity(AddBuildingDto dto)
    {
        return new Building(
            dto.Name, dto.Color, dto.Height, dto.Length, dto.Width,
            dto.X, dto.Y, dto.Z, dto.AreaId);
    }

    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
    }
}
