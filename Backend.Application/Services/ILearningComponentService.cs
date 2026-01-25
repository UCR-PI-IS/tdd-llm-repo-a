using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

public interface ILearningComponentService
{
    Task<IEnumerable<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);
}
