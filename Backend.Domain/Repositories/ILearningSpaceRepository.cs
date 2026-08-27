using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for creating and managing learning spaces.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Adds a new learning space to the repository.
    /// </summary>
    /// <param name="learningSpace">The learning space to add.</param>
    Task AddAsync(LearningSpace learningSpace);
}