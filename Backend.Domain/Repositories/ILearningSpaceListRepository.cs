using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

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
    /// Lists all learning components for the specified learning space by id.
    /// </summary>
    List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
}
