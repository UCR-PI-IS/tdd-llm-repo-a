using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning component data.
/// </summary>
internal class LearningComponentService : ILearningComponentListService
{
    private readonly ILearningComponentListRepository _learningComponentListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="learningComponentListRepository">The learning component repository dependency.</param>
    public LearningComponentService(ILearningComponentListRepository learningComponentListRepository)
    {
        _learningComponentListRepository = learningComponentListRepository;
    }

    /// <summary>
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null or empty.</exception>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));

        return await _learningComponentListRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }
}
