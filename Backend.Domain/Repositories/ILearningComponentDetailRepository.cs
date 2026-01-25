using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for managing LearningComponentDetail entities.
/// </summary>
public interface ILearningComponentDetailRepository
{
    /// <summary>
    /// Creates a new learning component detail.
    /// </summary>
    /// <param name="detail">The learning component detail to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateAsync(LearningComponentDetail detail);

    /// <summary>
    /// Gets a learning component detail by its identifier.
    /// </summary>
    /// <param name="detailId">The unique identifier of the detail.</param>
    /// <returns>The learning component detail, or null if not found.</returns>
    Task<LearningComponentDetail?> GetByIdAsync(Guid detailId);

    /// <summary>
    /// Gets all learning component details for a specific component.
    /// </summary>
    /// <param name="componentId">The identifier of the learning component.</param>
    /// <returns>A collection of learning component details.</returns>
    Task<IEnumerable<LearningComponentDetail>> GetByComponentIdAsync(Guid componentId);
}
