using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for LearningComponent entity operations.
/// </summary>
public interface ILearningComponentRepository
{
    /// <summary>
    /// Retrieves all learning components associated with a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>A collection of learning components in the specified learning space</returns>
    Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId);
}
