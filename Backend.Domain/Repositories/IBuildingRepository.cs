using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for building-related operations.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Adds a new building to the data store.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The added building.</returns>
    Task<Building> AddAsync(Building building);

    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists; otherwise, false.</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Checks if an area with the specified identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if the area exists; otherwise, false.</returns>
    Task<bool> AreaExistsAsync(int areaId) => Task.FromResult(true);
}
