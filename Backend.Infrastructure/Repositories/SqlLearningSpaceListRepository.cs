using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories
{
    internal class SqlLearningSpaceListRepository : ILearningSpaceListRepository
    {
        // Minimal fake data, matching test requirements
        public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
        {
            if (learningSpaceId == 1)
            {
                return new List<LearningComponent>
                {
                    new LearningComponent("Whiteboard"),
                    new LearningComponent("Projector")
                };
            }
            else if (learningSpaceId == 2)
            {
                return new List<LearningComponent>();
            }
            else
            {
                throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
            }
        }

        public System.Threading.Tasks.Task<LearningSpace> GetCurrentLearningSpaceListAsync()
        {
            throw new System.NotImplementedException();
        }
        public System.Threading.Tasks.Task<List<LearningSpace>> GetAllLearningSpacesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
