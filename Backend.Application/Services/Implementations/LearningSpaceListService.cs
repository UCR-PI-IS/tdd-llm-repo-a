using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;
using System;

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

        // Added for listing components in a learning space
        public List<LearningComponent> ListComponents(int learningSpaceId)
        {
            // Delegate to repository. Exception handling is at repo level (tests require exceptions surface naturally here).
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
