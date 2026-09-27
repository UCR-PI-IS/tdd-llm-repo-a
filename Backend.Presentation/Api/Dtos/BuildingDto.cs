namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents a building data transfer object for the building list.
/// </summary>
/// <param name="Id">The building identifier.</param>
/// <param name="Name">The name of the building.</param>
public record class BuildingDto(
    int Id,
    string Name);
