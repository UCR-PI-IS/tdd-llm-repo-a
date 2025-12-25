using UCR.ECCI.IS.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.IS.ThemePark.Backend.Application.Services.Implementations;

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