namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Response returned when a building is successfully updated.
/// </summary>
public class UpdateBuildingResponse
{
    /// <summary>
    /// Gets a value indicating whether the update was successful.
    /// </summary>
    public bool Success { get; set; } = true;
}
