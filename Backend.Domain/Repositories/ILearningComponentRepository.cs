using System.Collections.Generic;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

public interface ILearningComponentRepository
{
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
