using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful building creation.
/// </summary>
public class AddBuildingResponse
{
    /// <summary>
    /// Gets the created building data.
    /// </summary>
    public BuildingDto Building { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResponse"/> class with building data.
    /// </summary>
    /// <param name="building">The created building.</param>
    public AddBuildingResponse(BuildingDto building)
    {
        Building = building;
    }

    /// <summary>
    /// Creates an <see cref="AddBuildingResponse"/> from a domain <see cref="Building"/> entity.
    /// </summary>
    /// <param name="building">The building entity.</param>
    /// <returns>A new response instance.</returns>
    public static AddBuildingResponse FromBuilding(Building building)
    {
        return new AddBuildingResponse(new BuildingDto(building.InternalId, building.Name));
    }
}
