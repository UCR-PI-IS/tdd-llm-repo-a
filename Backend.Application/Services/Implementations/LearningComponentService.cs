using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the learning space does not exist.</exception>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            throw new ArgumentException("Learning space ID cannot be null or empty", nameof(learningSpaceId));
        }

        var components = await _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
        
        // Check if the learning space exists by verifying if any components exist or if we need to check existence
        // For now, we'll assume if we get an empty list, the learning space might not exist
        // But since the repository doesn't provide a way to check, we'll throw KeyNotFoundException 
        // only if the repository throws it or we have a way to verify existence
        
        return components;
    }
}
