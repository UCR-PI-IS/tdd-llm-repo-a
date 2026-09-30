namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Request DTO for adding a new building.
/// </summary>
/// <param name="Name">The name of the building.</param>
/// <param name="Color">The color of the building.</param>
/// <param name="Height">The height of the building in meters.</param>
/// <param name="Length">The length of the building in meters.</param>
/// <param name="Width">The width of the building in meters.</param>
/// <param name="X">The X coordinate of the building.</param>
/// <param name="Y">The Y coordinate of the building.</param>
/// <param name="Z">The Z coordinate of the building.</param>
/// <param name="AreaId">The area identifier that this building belongs to.</param>
public record class AddBuildingRequest(
    string Name,
    string Color,
    float Height,
    float Length,
    float Width,
    float X,
    float Y,
    float Z,
    int AreaId);
