using System.Collections.Generic;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

public interface ILearningComponentService
{
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
