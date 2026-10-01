namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating a building.
/// </summary>
/// <param name="Name">Name of the building.</param>
/// <param name="Color">Color of the building.</param>
/// <param name="Height">Height of the building in meters.</param>
/// <param name="Length">Length of the building in meters.</param>
/// <param name="Width">Width of the building in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
public record class UpdateBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z);
