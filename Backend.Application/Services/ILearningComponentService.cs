using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages learning component data.
/// </summary>
public interface ILearningComponentService
{
    /// <summary>
    /// Retrieves a list of learning components for a given learning space.
    /// </summary>
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
