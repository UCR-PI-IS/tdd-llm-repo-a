using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for accessing learning component data.
/// </summary>
public interface ILearningComponentRepository
{
    /// <summary>
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The ID of the learning space.</param>
    /// <returns>A collection of learning components.</returns>
    Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId);
}
