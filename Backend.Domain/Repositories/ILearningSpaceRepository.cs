using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for learning space operations.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Adds a new learning space to the repository.
    /// </summary>
    /// <param name="learningSpace">The learning space to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(LearningSpace learningSpace);
}
