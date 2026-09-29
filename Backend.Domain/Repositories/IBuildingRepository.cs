using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for building persistence operations.
/// </summary>
public interface IBuildingRepository
{
    /// <summary>
    /// Adds a new building to the data source.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    Task AddAsync(Building building);

    /// <summary>
    /// Checks whether a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the same name exists; otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name);
}
