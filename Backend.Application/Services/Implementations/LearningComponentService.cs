using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

public class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _repository;

    public LearningComponentService(ILearningComponentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (learningSpaceId == null)
        {
            throw new ArgumentNullException(nameof(learningSpaceId));
        }

        if (string.IsNullOrEmpty(learningSpaceId))
        {
            throw new ArgumentException("Learning space ID cannot be empty", nameof(learningSpaceId));
        }

        return await _repository.GetByLearningSpaceIdAsync(learningSpaceId);
    }
}
