using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a newly created building.
/// </summary>
/// <param name="Building">The building entity that was created.</param>
public record class AddBuildingResponse(Building Building);
