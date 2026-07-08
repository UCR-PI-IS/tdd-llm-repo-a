using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning component data.
/// </summary>
internal class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _learningComponentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentRepository">The learning component repository dependency.</param>
    public LearningComponentService(ILearningComponentRepository learningComponentRepository)
    {
        _learningComponentRepository = learningComponentRepository;
    }

    /// <summary>
    /// Retrieves a list of learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="learningSpaceId"/> is null or empty.</exception>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(String learningSpaceId)
    {
        if (String.IsNullOrEmpty(learningSpaceId))
        {
            throw new ArgumentException("Learning space ID cannot be null or empty", nameof(learningSpaceId));
        }

        return await _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }
}
