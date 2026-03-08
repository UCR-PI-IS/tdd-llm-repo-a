using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for learning space operations.
/// </summary>
public interface ILearningSpaceRepository
{
    /// <summary>
    /// Gets a learning space by its identifier.
    /// </summary>
    /// <param name="id">The learning space identifier</param>
    /// <returns>The learning space if found, null otherwise</returns>
    Task<LearningSpace?> GetByIdAsync(string id);
}
