using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

public interface ILearningComponentService
{
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(Guid learningSpaceId);
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
