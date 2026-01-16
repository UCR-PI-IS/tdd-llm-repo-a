using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services
{
    /// <summary>
    /// Interface for the service that manages learning space components data.
    /// </summary>
    public interface ILearningSpaceListService
    {
        /// <summary>
        /// Lists the learning components of a given learning space id.
        /// </summary>
        /// <param name="learningSpaceId">The learning space identifier.</param>
        /// <returns>List of components.</returns>
        List<LearningComponent> ListComponents(int learningSpaceId);
    }
}
