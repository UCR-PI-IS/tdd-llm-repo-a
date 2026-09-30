using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a successfully created building.
/// </summary>
/// <param name="Building">The created building entity.</param>
public record AddBuildingResponse(Building Building);
