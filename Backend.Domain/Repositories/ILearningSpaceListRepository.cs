using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// Retrieves the components that belong to the given learning space id.
    /// Implementations may throw InvalidLearningSpaceException when the id is not valid.
    /// </summary>
    List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
}
