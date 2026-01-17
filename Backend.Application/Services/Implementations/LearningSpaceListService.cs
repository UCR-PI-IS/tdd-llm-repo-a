using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations
{
    /// <summary>
    /// Service implementation for retrieving learning space data.
    /// </summary>
    internal class LearningSpaceListService : ILearningSpaceListService
    {
        private readonly ILearningSpaceListRepository _learningSpaceListRepository;

        public LearningSpaceListService(ILearningSpaceListRepository learningSpaceListRepository)
        {
            _learningSpaceListRepository = learningSpaceListRepository;
        }

        public List<LearningComponent> ListComponents(int learningSpaceId)
        {
            // Directly pass through to repository, propagate exception and result
            return _learningSpaceListRepository.GetComponentsByLearningSpaceId(learningSpaceId);
        }

        public System.Threading.Tasks.Task<LearningSpace> GetCurrentLearningSpaceListAsync()
        {
            return _learningSpaceListRepository.GetCurrentLearningSpaceListAsync();
        }

        public System.Threading.Tasks.Task<List<LearningSpace>> GetAllLearningSpacesAsync()
        {
            return _learningSpaceListRepository.GetAllLearningSpacesAsync();
        }
    }
}
