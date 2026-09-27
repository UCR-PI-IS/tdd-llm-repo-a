namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a building.
/// </summary>
/// <param name="Id">The unique identifier of the building.</param>
/// <param name="Name">The name of the building.</param>
public record class BuildingDto(int Id, string Name);
