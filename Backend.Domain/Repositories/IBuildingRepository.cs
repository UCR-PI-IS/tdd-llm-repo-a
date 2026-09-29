using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for building operations.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    Task<Building> AddAsync(Building building);

    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if the building exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Checks if any building exists in the specified area.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if a building exists in the area; otherwise, false.</returns>
    Task<bool> ExistsByAreaIdAsync(int areaId);
}
