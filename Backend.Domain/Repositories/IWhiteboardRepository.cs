using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for whiteboard operations.
/// </summary>
public interface IWhiteboardRepository
{
    /// <summary>
    /// Adds a new whiteboard to the repository.
    /// </summary>
    /// <param name="whiteboard">The whiteboard to add</param>
    Task AddAsync(Whiteboard whiteboard);

    /// <summary>
    /// Gets a whiteboard by its identifier.
    /// </summary>
    /// <param name="id">The whiteboard identifier</param>
    /// <returns>The whiteboard if found, null otherwise</returns>
    Task<Whiteboard?> GetByIdAsync(string id);
}
