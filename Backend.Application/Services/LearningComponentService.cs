using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));
        }

        return await _repository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }
}
