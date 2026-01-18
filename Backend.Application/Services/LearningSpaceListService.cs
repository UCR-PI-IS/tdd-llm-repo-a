using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Legacy-style application service used by existing unit tests.
/// Provides a synchronous API to list components of a learning space.
/// </summary>
public class LearningSpaceListService
{
    private readonly ILearningSpaceListRepository _repository;

    public LearningSpaceListService(ILearningSpaceListRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lists the components that belong to the specified learning space id.
    /// Delegates to the repository which may throw InvalidLearningSpaceException for invalid ids.
    /// </summary>
    public List<LearningComponent> ListComponents(int learningSpaceId)
    {
        return _repository.GetComponentsByLearningSpaceId(learningSpaceId);
    }
}
