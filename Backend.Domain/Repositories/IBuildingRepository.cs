using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for building write operations.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Persists a new building to the data store.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    Task<Building> AddAsync(Building building);

    /// <summary>
    /// Checks whether a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists; otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Checks whether an area with the specified identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if the area exists; otherwise false.</returns>
    Task<bool> AreaExistsAsync(int areaId);

    /// <summary>
    /// Retrieves a building by its internal identifier.
    /// </summary>
    /// <param name="id">The building's internal identifier.</param>
    /// <returns>The building entity if found; otherwise null.</returns>
    Task<Building?> GetByIdAsync(int id);

    /// <summary>
    /// Persists updates to an existing building in the data store.
    /// </summary>
    /// <param name="building">The building entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Building building);
}
