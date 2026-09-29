using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages building creation.
/// </summary>
public interface IBuildingService
{
    /// <summary>
    /// Adds a new building to the system.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    Task<Building> AddBuildingAsync(Building building);
}
