using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for managing buildings.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> AddAsync(Building building);

    /// <summary>
    /// Retrieves a building by name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The building if found, null otherwise.</returns>
    Task<Building?> GetByNameAsync(string name);
}
