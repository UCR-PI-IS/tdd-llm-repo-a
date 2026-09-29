using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful building creation.
/// </summary>
public class AddBuildingResponse
{
    /// <summary>
    /// Gets the created building.
    /// </summary>
    public Building Building { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResponse"/> class.
    /// </summary>
    /// <param name="building">The created building entity.</param>
    public AddBuildingResponse(Building building)
    {
        Building = building;
    }
}
