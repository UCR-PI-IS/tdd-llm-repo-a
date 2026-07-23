using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

public interface ILearningComponentRepository
{
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(Guid learningSpaceId);
}
