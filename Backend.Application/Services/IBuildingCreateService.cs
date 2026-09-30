using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for building creation operations.
/// </summary>
public interface IBuildingCreateService
{
    /// <summary>
    /// Adds a new building to the system after validating business rules.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    Task<Building> AddBuildingAsync(Building building);
}
