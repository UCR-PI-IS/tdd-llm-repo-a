using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for persisting learning space entities.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Adds a new learning space to the data store.
    /// </summary>
    /// <param name="learningSpace">The learning space entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(LearningSpace learningSpace);
}
