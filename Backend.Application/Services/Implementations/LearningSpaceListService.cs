using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations
{
    /// <summary>
    /// Service implementation for retrieving learning space data.
    /// </summary>
    internal class LearningSpaceListService : ILearningSpaceListService
    {
        private readonly ILearningSpaceListRepository _learningSpaceListRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearningSpaceListService"/> class.
        /// </summary>
        /// <param name="learningSpaceListRepository">The learning space repository dependency.</param>
        public LearningSpaceListService(ILearningSpaceListRepository learningSpaceListRepository)
        {
            _learningSpaceListRepository = learningSpaceListRepository;
        }

        /// <summary>
        /// Lists the learning components of a given learning space id.
        /// </summary>
        /// <param name="learningSpaceId">The learning space identifier.</param>
        /// <returns>List of components.</returns>
        public List<LearningComponent> ListComponents(int learningSpaceId)
        {
            return _learningSpaceListRepository.GetComponentsByLearningSpaceId(learningSpaceId);
        }
    }
}
