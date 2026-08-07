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
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null or empty.</exception>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(String learningSpaceId)
    {
        if (String.IsNullOrEmpty(learningSpaceId))
        {
            throw new ArgumentException("Learning space ID cannot be null or empty.", "learningSpaceId");
        }

        var components = await _learningComponentRepository.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
        
        // Check if learning space exists by verifying if the space ID is valid
        // For this implementation, if no components are found, we return an empty list
        // The existence of the learning space should be checked by a separate service/repository
        
        return components;
    }
}
