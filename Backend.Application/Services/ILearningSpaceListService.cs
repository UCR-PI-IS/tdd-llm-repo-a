using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages learning space data.
/// </summary>
public interface ILearningSpaceListService
{
    /// <summary>
    /// Retrieves the current learning space (e.g., the one selected or predefined).
    /// </summary>
    /// <returns>A single learning space entity.</returns>
    Task<LearningSpace> GetCurrentLearningSpaceListAsync();

    /// <summary>
    /// Retrieves a list of all learning spaces available in the database.
    /// </summary>
    /// <returns>A list of learning space entities.</returns>
    Task<List<LearningSpace>> GetAllLearningSpacesAsync();
}