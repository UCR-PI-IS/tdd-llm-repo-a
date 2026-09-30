namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for adding a building.
/// </summary>
/// <param name="Name">The name of the building.</param>
/// <param name="Color">The color of the building.</param>
/// <param name="Height">Height of the building in meters.</param>
/// <param name="Length">Length of the building in meters.</param>
/// <param name="Width">Width of the building in meters.</param>
/// <param name="X">X coordinate of the building.</param>
/// <param name="Y">Y coordinate of the building.</param>
/// <param name="Z">Z coordinate of the building.</param>
/// <param name="AreaId">The area identifier the building belongs to.</param>
public record class AddBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z,
    int AreaId);
