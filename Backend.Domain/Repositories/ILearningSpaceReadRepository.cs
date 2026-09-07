using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for reading learning space data from the data source.
/// </summary>
public interface ILearningSpaceReadRepository
{
    /// <summary>
    /// Retrieves a learning space by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the learning space.</param>
    /// <returns>The learning space if found; otherwise, null.</returns>
    Task<LearningSpace?> GetByIdAsync(string id);
}
