using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing learning space data sources.
/// </summary>
public interface ILearningSpaceListRepository
{
    /// <summary>
    /// Retrieves the current learning space.
    /// </summary>
    Task<LearningSpace> GetCurrentLearningSpaceListAsync();

    /// <summary>
    /// Retrieves all learning spaces.
    /// </summary>
    Task<List<LearningSpace>> GetAllLearningSpacesAsync();

    /// <summary>
    /// Retrieves a learning space by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the learning space.</param>
    /// <returns>The learning space if found; otherwise, null.</returns>
    Task<LearningSpace?> GetByIdAsync(string id);
}
