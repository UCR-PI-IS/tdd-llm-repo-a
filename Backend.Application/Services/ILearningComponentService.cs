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
    /// Creates a new learning component, generating a unique ID if none is provided.
    /// </summary>
    /// <param name="request">The creation request containing component parameters.</param>
    /// <returns>The created learning component entity.</returns>
    /// <exception cref="Domain.Exceptions.ValidationException">Thrown when the request contains invalid data.</exception>
    /// <exception cref="Domain.Exceptions.DuplicateIdException">Thrown when the provided ID already exists.</exception>
    Task<LearningComponent> CreateComponentAsync(CreateComponentRequest request);
}
