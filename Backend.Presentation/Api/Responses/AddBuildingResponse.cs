using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a successfully added building.
/// </summary>
/// <param name="Building">The building data that was added.</param>
public record class AddBuildingResponse(BuildingDto Building);
