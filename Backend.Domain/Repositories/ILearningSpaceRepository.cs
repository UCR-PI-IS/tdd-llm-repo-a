using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for creating learning spaces in the data source.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Adds a new learning space to the data source.
    /// </summary>
    /// <param name="learningSpace">The learning space entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(LearningSpace learningSpace);

    /// <summary>
    /// Retrieves a learning space by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the learning space.</param>
    /// <returns>The learning space if found; otherwise, null.</returns>
    Task<LearningSpace?> GetByIdAsync(string id);
}
