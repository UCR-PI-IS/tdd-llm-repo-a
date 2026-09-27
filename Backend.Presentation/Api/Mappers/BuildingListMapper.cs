using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class BuildingListMapper
{
    public static GetBuildingListResponse ToResponse(List<Building> buildings)
    {
        var dtos = buildings.Select(b => new BuildingDto(b.InternalId, b.Name)).ToList();
        return new GetBuildingListResponse(dtos);
    }
}
