using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for persisting whiteboard entities.
/// </summary>
public interface IWhiteboardRepository
{
    /// <summary>
    /// Adds a new whiteboard to the data source.
    /// </summary>
    /// <param name="whiteboard">The whiteboard entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Whiteboard whiteboard);

    /// <summary>
    /// Retrieves a whiteboard by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the whiteboard.</param>
    /// <returns>The whiteboard if found; otherwise, null.</returns>
    Task<Whiteboard?> GetByIdAsync(string id);

    /// <summary>
    /// Updates an existing whiteboard in the data source.
    /// </summary>
    /// <param name="whiteboard">The whiteboard entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Whiteboard whiteboard);

    /// <summary>
    /// Retrieves all whiteboards that belong to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of whiteboards belonging to the learning space.</returns>
    Task<List<Whiteboard>> GetByLearningSpaceIdAsync(string learningSpaceId);
}
