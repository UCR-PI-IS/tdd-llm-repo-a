using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response returned when a building is successfully added.
/// </summary>
public class AddBuildingResponse
{
    /// <summary>
    /// The building that was added.
    /// </summary>
    public Building Building { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddBuildingResponse"/> class.
    /// </summary>
    /// <param name="building">The added building.</param>
    public AddBuildingResponse(Building building)
    {
        Building = building;
    }
}
