using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages learning space data.
/// </summary>
public interface ILearningSpaceListService
{
    /// <summary>
    /// Lists learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <returns>List of learning components; throws if the space id is invalid.</returns>
    List<LearningComponent> ListComponents(int learningSpaceId);

    /// <summary>
    /// Retrieves the current learning space (e.g., the one selected or predefined).
    /// </summary>
    /// <returns>A single learning space entity.</returns>
    Task<LearningSpace> GetCurrentLearningSpaceListAsync();

    /// <summary>
    /// Retrieves a list of all learning spaces available in the database.
    /// </summary>
    /// <returns>A list of learning space entities.</returns>
    Task<List<LearningSpace>> GetAllLearningSpacesAsync();
}
