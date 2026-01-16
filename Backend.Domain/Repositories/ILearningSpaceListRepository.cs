using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories
{
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
        /// Gets the components by learning space identifier.
        /// </summary>
        /// <param name="learningSpaceId">The learning space identifier.</param>
        /// <returns>List of components.</returns>
        List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId);
    }
}
