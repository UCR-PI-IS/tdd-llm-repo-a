using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for managing LearningComponent entities.
/// </summary>
public interface ILearningComponentRepository
{
    /// <summary>
    /// Gets all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A collection of learning components.</returns>
    Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId);

    /// <summary>
    /// Creates a new learning component.
    /// </summary>
    /// <param name="component">The learning component to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(LearningComponent component);

    /// <summary>
    /// Gets a learning component by its identifier.
    /// </summary>
    /// <param name="componentId">The unique identifier of the component.</param>
    /// <returns>The learning component, or null if not found.</returns>
    Task<LearningComponent?> GetByIdAsync(Guid componentId);
}
