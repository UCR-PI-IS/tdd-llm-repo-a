using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

public class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _repository;

    public LearningComponentService(ILearningComponentRepository repository)
    {
        _repository = repository;
    }

    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(Guid learningSpaceId)
    {
        EnsureValidGuid(learningSpaceId);
        return _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }

    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        var parsedId = ParseLearningSpaceId(learningSpaceId);
        return _repository.GetComponentsByLearningSpaceIdAsync(parsedId);
    }

    private static void EnsureValidGuid(Guid learningSpaceId)
    {
        if (learningSpaceId == Guid.Empty)
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));
    }

    private static Guid ParseLearningSpaceId(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));

        if (!Guid.TryParse(learningSpaceId, out var parsedId) || parsedId == Guid.Empty)
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));

        return parsedId;
    }
}
