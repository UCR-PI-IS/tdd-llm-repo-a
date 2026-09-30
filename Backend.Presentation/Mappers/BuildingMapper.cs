using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class BuildingMapper
{
    public static List<BuildingDto> ToDtoList(IEnumerable<Building> buildings)
    {
        return buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
    }

    public static Building ToEntity(AddBuildingRequest request)
    {
        return new Building(
            0,
            request.Name,
            request.Color,
            request.Height,
            request.Length,
            request.Width,
            request.X,
            request.Y,
            request.Z,
            request.AreaId);
    }

    public static AddBuildingResponse ToAddResponse(Building building)
    {
        return new AddBuildingResponse(new BuildingDto(building.InternalId, building.Name));
    }
}
