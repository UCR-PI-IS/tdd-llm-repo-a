namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new building.
/// </summary>
public record class CreateBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z,
    int AreaId)
{
    /// <summary>
    /// Parameterless constructor for model binding fallback.
    /// </summary>
    public CreateBuildingDto() : this("Test Building", "Red", 10.0f, 10.0f, 10.0f, 0.0f, 0.0f, 0.0f, 1)
    {
    }
}
