using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for accessing learning space and components data sources.
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
    /// Retrieves the list of components for a specific learning space id.
    /// Throws InvalidLearningSpaceException if invalid.
    /// </summary>
    List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
}
