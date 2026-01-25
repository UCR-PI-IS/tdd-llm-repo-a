using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for learning component operations.
/// </summary>
public class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _repository;

    /// <summary>
    /// Constructor for LearningComponentService.
    /// </summary>
    /// <param name="repository">The learning component repository</param>
    public LearningComponentService(ILearningComponentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves all components in a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>A collection of learning components</returns>
    /// <exception cref="ArgumentNullException">Thrown when learningSpaceId is null</exception>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is empty</exception>
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
