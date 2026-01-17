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
    /// (NEW) Retrieves the components for a given learning space.
    /// Returns a list for valid id, empty if none, throws exception if not valid.
    /// </summary>
    List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
}
