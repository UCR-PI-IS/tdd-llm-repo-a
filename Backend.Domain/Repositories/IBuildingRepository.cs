using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for building repository operations supporting the add-building workflow.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Adds a new building to the data source.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The persisted building entity.</returns>
    Task<Building> AddAsync(Building building);

    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// Checks if an area with the specified identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if the area exists, otherwise false.</returns>
    Task<bool> AreaExistsAsync(int areaId);
}
