using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages learning component data.
/// </summary>
public interface ILearningComponentService
{
    /// <summary>
    /// Retrieves all learning components that belong to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    /// <exception cref="ArgumentException">Thrown when learningSpaceId is null or empty.</exception>
    Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId);

    /// <summary>
    /// Creates a new learning component.
    /// </summary>
    /// <param name="request">The request containing the component details.</param>
    /// <returns>The created learning component.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    /// <exception cref="DuplicateIdException">Thrown when a duplicate ID is provided.</exception>
    Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request);
}
