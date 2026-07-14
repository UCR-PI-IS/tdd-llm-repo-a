using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing learning component data sources.
/// </summary>
public interface ILearningComponentRepository
{
    /// <summary>
    /// Retrieves all learning components for a given learning space.
    /// </summary>
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
