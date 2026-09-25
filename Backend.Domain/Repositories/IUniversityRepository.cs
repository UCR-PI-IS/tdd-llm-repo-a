using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for university repository operations.
/// </summary>
public interface IUniversityRepository
{
    /// <summary>
    /// Adds a new university to the data source.
    /// </summary>
    /// <param name="university">The university entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(University university);

    /// <summary>
    /// Checks if a university with the specified name already exists.
    /// </summary>
    /// <param name="name">The university name to check.</param>
    /// <returns>True if a university with the name exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name);
}
