using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for building service operations.
/// </summary>
public interface IBuildingService
{
    /// <summary>
    /// Adds a new building to the system after validating uniqueness and area existence.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The created building entity.</returns>
    Task<Building> AddBuildingAsync(Building building);
}
