using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
    /// Creates a new learning space with the specified parameters.
    /// </summary>
    /// <param name="type">The type of the learning space (Classroom, Auditorium, or Laboratory).</param>
    /// <param name="height">The height of the learning space in meters.</param>
    /// <param name="width">The width of the learning space in meters.</param>
    /// <param name="length">The length of the learning space in meters.</param>
    /// <returns>The created learning space entity with generated ID.</returns>
    public async Task<LearningSpace> CreateLearningSpaceAsync(string type, float height, float width, float length)
    {
        // Create the learning space entity (validation happens in constructor)
        var learningSpace = new LearningSpace(type, height, width, length);

        // Persist to repository
        await _learningSpaceRepository.AddAsync(learningSpace);

        return learningSpace;
    }
}
