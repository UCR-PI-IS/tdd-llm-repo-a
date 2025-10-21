using UCR.ECCI.PI.ThemePark.Frontend.Blazor.Domain.Entities.ComponentsManagement;

namespace UCR.ECCI.PI.ThemePark.Frontend.Blazor.Application.Services;

/// <summary>
/// Interface that defines the service operations for managing learning components.
/// </summary>
public interface ILearningComponentServices
{
    /// <summary>
    /// Retrieves all learning components available in the system.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, which returns a list of all learning components 
    /// (such as whiteboards and projectors).
    /// </returns>
    public Task<IEnumerable<LearningComponent>> GetLearningComponentAsync();

    /// <summary>
    /// Retrieves learning components that belong to a specific learning space, identified by building, floor, and space ID.
    /// </summary>
/// <param name="learningSpaceId">The identifier of the specific learning space (e.g., a classroom or lab).</param>
    /// <returns>
    /// A task representing the asynchronous operation, which returns the list of components associated with the specified space.
    /// </returns>
    public Task<IEnumerable<LearningComponent>> GetLearningComponentsByIdAsync(int learningSpaceId);

    /// <summary>
    /// Retrieves a single learning component by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the learning component to retrieve.</param>
    /// <returns>
    /// A task representing the asynchronous operation, which returns the learning component
    /// that matches the specified identifier.
    /// </returns>
    public Task<LearningComponent> GetSingleLearningComponentByIdAsync(int id);

    /// <summary>
    /// Deletes a learning component from the system by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the learning component to be deleted.</param>
    /// <returns>
    /// A task representing the asynchronous operation, which returns a boolean value indicating
    /// whether the deletion was successful.
    /// </returns>
    public Task<bool> DeleteLearningComponentAsync(int id);
    
    public Task<bool> UpdateLearningComponentAsync(int learningSpaceId, LearningComponent learningComponent);
}