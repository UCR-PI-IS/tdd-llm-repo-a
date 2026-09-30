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
    /// <param name="building">The building entity to add.</param>
    /// <param name="areaId">The ID of the area where the building is located.</param>
    /// <returns>The added building entity.</returns>
    Task<Building> AddBuildingAsync(Building building, int areaId);
}
