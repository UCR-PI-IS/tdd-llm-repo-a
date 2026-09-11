using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing learning component data sources.
/// </summary>
public interface ILearningComponentRepository
{
    /// <summary>
    /// Retrieves all learning components that belong to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);

    /// <summary>
    /// Checks if a learning component with the specified ID exists.
    /// </summary>
    /// <param name="componentId">The identifier of the component.</param>
    /// <returns>True if the component exists, false otherwise.</returns>
    Task<bool> ExistsAsync(string componentId);

    /// <summary>
    /// Adds a new learning component to the data source.
    /// </summary>
    /// <param name="component">The component to add.</param>
    Task AddAsync(LearningComponent component);
}
