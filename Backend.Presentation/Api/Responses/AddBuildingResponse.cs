using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response returned after successfully adding a building.
/// </summary>
/// <param name="Building">The building that was added.</param>
public record class AddBuildingResponse(Building Building);
