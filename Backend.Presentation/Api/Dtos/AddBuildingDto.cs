namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for adding a new building.
/// </summary>
/// <param name="Name">The name of the building.</param>
/// <param name="Color">The color of the building.</param>
/// <param name="Height">The height in meters.</param>
/// <param name="Length">The length in meters.</param>
/// <param name="Width">The width in meters.</param>
/// <param name="X">The X coordinate.</param>
/// <param name="Y">The Y coordinate.</param>
/// <param name="Z">The Z coordinate.</param>
/// <param name="AreaId">The area identifier.</param>
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
