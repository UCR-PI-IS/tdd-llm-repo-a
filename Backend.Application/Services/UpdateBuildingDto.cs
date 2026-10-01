namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object used to update an existing building.
/// </summary>
/// <param name="Name">Name of the building.</param>
/// <param name="Color">Color of the building.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Length">Length in meters.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
public record class UpdateBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z);
