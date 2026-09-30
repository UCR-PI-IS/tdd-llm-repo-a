using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a successfully created building.
/// </summary>
public class AddBuildingResponse
{
    /// <summary>
    /// Gets the building entity that was created.
    /// </summary>
    public Building Building { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResponse"/> class.
    /// </summary>
    /// <param name="building">The building entity.</param>
    public AddBuildingResponse(Building building)
    {
        Building = building;
    }
}
