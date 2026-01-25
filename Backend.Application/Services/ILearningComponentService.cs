using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for managing learning component operations.
/// </summary>
public interface ILearningComponentService
{
    /// <summary>
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A collection of learning components.</returns>
    /// <exception cref="ArgumentNullException">Thrown when learningSpaceId is null.</exception>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is empty.</exception>
    Task<IEnumerable<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
