namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new building.
/// </summary>
/// <param name="Name">The name of the building.</param>
/// <param name="Color">The color of the building.</param>
/// <param name="Height">The height of the building in meters.</param>
/// <param name="Length">The length of the building in meters.</param>
/// <param name="Width">The width of the building in meters.</param>
/// <param name="X">The X coordinate of the building.</param>
/// <param name="Y">The Y coordinate of the building.</param>
/// <param name="Z">The Z coordinate of the building.</param>
/// <param name="AreaId">The ID of the area where the building is located.</param>
public record class CreateBuildingDto(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z,
    int AreaId);
