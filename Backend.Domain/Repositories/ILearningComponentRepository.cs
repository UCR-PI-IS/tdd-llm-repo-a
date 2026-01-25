using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

public interface ILearningComponentRepository
{
    Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId);
}
