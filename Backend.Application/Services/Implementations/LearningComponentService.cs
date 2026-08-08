using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning component data.
/// </summary>
public class LearningComponentService : ILearningComponentService
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
    /// Retrieves all learning components belonging to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of <see cref="LearningComponent"/> entities.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null, empty, or whitespace.</exception>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));

        return await _learningComponentListRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
    }
}
