namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a successfully updated building.
/// </summary>
/// <param name="Id">The internal identifier of the updated building.</param>
/// <param name="Name">The updated name of the building.</param>
/// <param name="Color">The updated color of the building.</param>
public record class UpdateBuildingResponse(int Id, string Name, string Color);
