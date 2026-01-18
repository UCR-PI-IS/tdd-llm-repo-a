using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning components in a space.
/// </summary>
internal class LearningSpaceListService : ILearningSpaceListService
{
    private readonly ILearningSpaceListRepository _learningSpaceListRepository;

    public LearningSpaceListService(ILearningSpaceListRepository learningSpaceListRepository)
    {
        _learningSpaceListRepository = learningSpaceListRepository;
    }

    /// <summary>
    /// Lists learning components for the specified learning space ID.
    /// </summary>
    public List<LearningComponent> ListComponents(int learningSpaceId)
    {
        return _learningSpaceListRepository.GetComponentsByLearningSpaceId(learningSpaceId);
    }

    public Task<LearningSpace> GetCurrentLearningSpaceListAsync()
    {
        return _learningSpaceListRepository.GetCurrentLearningSpaceListAsync();
    }

    public Task<List<LearningSpace>> GetAllLearningSpacesAsync()
    {
        return _learningSpaceListRepository.GetAllLearningSpacesAsync();
    }
}
