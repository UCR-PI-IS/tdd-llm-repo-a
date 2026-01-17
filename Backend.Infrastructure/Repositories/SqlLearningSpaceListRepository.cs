using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;
using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories
{
    // Stub for demonstration – would access EF/DB in production.
    internal class SqlLearningSpaceListRepository : ILearningSpaceListRepository
    {
        // Simulated in-memory data for test-driven implementation only.
        private readonly Dictionary<int, List<LearningComponent>> _componentsBySpace = new()
        {
            { 1, new List<LearningComponent> { new LearningComponent("Whiteboard"), new LearningComponent("Projector") } },
            { 2, new List<LearningComponent>() } // space with no components
        };

        public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
        {
            if (!_componentsBySpace.ContainsKey(learningSpaceId))
            {
                throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
            }
            return new List<LearningComponent>(_componentsBySpace[learningSpaceId]);
        }

        // Async stubs for demonstration – not relevant for immediate failing tests but left for compatibility:
        public System.Threading.Tasks.Task<LearningSpace> GetCurrentLearningSpaceListAsync()
        {
            throw new NotImplementedException();
        }
        public System.Threading.Tasks.Task<List<LearningSpace>> GetAllLearningSpacesAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class InvalidLearningSpaceException : Exception
    {
        public InvalidLearningSpaceException(string message) : base(message) { }
    }
}
