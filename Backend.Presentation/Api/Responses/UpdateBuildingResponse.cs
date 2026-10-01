using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Response object for a successfully updated building.
/// </summary>
public class UpdateBuildingResponse
{
    /// <summary>
    /// Gets the building entity that was updated.
    /// </summary>
    public Building Building { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuildingResponse"/> class.
    /// </summary>
    /// <param name="building">The building entity.</param>
    public UpdateBuildingResponse(Building building)
    {
        Building = building;
    }
}
