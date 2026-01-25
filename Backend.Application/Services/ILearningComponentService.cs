using UCR.ECCI.PI.ThemePark.Backend.Application.ViewModels;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for managing learning components.
/// </summary>
public interface ILearningComponentService
{
    /// <summary>
    /// Gets all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A collection of learning component view models.</returns>
    Task<IEnumerable<LearningComponentViewModel>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
