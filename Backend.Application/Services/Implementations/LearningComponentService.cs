using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning component data.
/// </summary>
public class LearningComponentService : ILearningComponentService
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
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>A collection of learning components in the specified learning space</returns>
    public async Task<IEnumerable<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (learningSpaceId == null)
            throw new ArgumentNullException(nameof(learningSpaceId));
            
        if (string.IsNullOrWhiteSpace(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be empty.", nameof(learningSpaceId));

        return await _learningComponentRepository.GetByLearningSpaceIdAsync(learningSpaceId);
    }
}
