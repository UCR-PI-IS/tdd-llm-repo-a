using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for building service operations.
/// </summary>
public interface IBuildingService
{
    /// <summary>
    /// Adds a new building to the system.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The added building.</returns>
    Task<Building> AddBuildingAsync(Building building);
}
