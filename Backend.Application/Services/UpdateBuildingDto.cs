namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating a building.
/// </summary>
/// <param name="Name">New name of the building.</param>
/// <param name="Color">New color of the building.</param>
/// <param name="Height">New height of the building in meters.</param>
/// <param name="Length">New length of the building in meters.</param>
/// <param name="Width">New width of the building in meters.</param>
/// <param name="X">New X coordinate.</param>
/// <param name="Y">New Y coordinate.</param>
/// <param name="Z">New Z coordinate.</param>
public record class UpdateBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z);
