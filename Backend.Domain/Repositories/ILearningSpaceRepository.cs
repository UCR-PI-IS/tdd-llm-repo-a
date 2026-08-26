using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for creating learning space entities.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Adds a new learning space to the repository.
    /// </summary>
    /// <param name="learningSpace">The learning space to add.</param>
    Task AddAsync(LearningSpace learningSpace);
}
