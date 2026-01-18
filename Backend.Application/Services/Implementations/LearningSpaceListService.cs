using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for retrieving learning space data.
/// </summary>
internal class LearningSpaceListService : ILearningSpaceListService
{
    private readonly ILearningSpaceListRepository _learningSpaceListRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningSpaceListService"/> class.
    /// </summary>
    /// <param name="learningSpaceListRepository">The learning space repository dependency.</param>
    public LearningSpaceListService(ILearningSpaceListRepository learningSpaceListRepository)
    {
        _learningSpaceListRepository = learningSpaceListRepository;
    }

    /// <summary>
    /// Retrieves a list of components belonging to the specified learning space ID.
    /// </summary>
    /// <param name="learningSpaceId">The ID of the learning space.</param>
    /// <returns>List of learning components.</returns>
    public List<LearningComponent> ListComponents(int learningSpaceId)
    {
        // Delegates to repository. Handles exception propagation, as test expects domain exception.
        return _learningSpaceListRepository.GetComponentsByLearningSpaceId(learningSpaceId);
    }

    /// <summary>
    /// Retrieves the current learning space (e.g., the one selected or predefined).
    /// </summary>
    /// <returns>A single learning space entity.</returns>
    public Task<LearningSpace> GetCurrentLearningSpaceListAsync()
    {
        return _learningSpaceListRepository.GetCurrentLearningSpaceListAsync();
    }

    /// <summary>
    /// Retrieves a list of all learning spaces available in the database.
    /// </summary>
    /// <returns>A list of learning space entities.</returns>
    public Task<List<LearningSpace>> GetAllLearningSpacesAsync()
    {
        return _learningSpaceListRepository.GetAllLearningSpacesAsync();
    }
}
