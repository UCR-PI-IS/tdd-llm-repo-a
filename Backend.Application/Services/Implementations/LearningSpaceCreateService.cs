using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating learning spaces.
/// </summary>
internal class LearningSpaceCreateService : ILearningSpaceCreateService
{
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningSpaceCreateService"/> class.
    /// </summary>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public LearningSpaceCreateService(ILearningSpaceRepository learningSpaceRepository)
    {
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Creates a new learning space with the specified parameters and persists it.
    /// </summary>
    /// <param name="type">Type of the learning space.</param>
    /// <param name="height">Height in meters.</param>
    /// <param name="width">Width in meters.</param>
    /// <param name="length">Length in meters.</param>
    /// <returns>The created learning space entity.</returns>
    public async Task<LearningSpace> CreateLearningSpaceAsync(string type, float height, float width, float length)
    {
        var learningSpace = new LearningSpace(type, height, width, length);
        await _learningSpaceRepository.AddAsync(learningSpace);
        return learningSpace;
    }
}
