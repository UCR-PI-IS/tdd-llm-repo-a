using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Implementation of the learning space creation service.
/// </summary>
public class LearningSpaceCreateService : ILearningSpaceCreateService
{
    private readonly ILearningSpaceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningSpaceCreateService"/> class.
    /// </summary>
    /// <param name="repository">The repository for persisting learning spaces.</param>
    public LearningSpaceCreateService(ILearningSpaceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates a new learning space with the specified parameters.
    /// </summary>
    /// <param name="type">The type of the learning space.</param>
    /// <param name="height">The height of the learning space.</param>
    /// <param name="width">The width of the learning space.</param>
    /// <param name="length">The length of the learning space.</param>
    /// <returns>The created learning space entity.</returns>
    public async Task<LearningSpace> CreateLearningSpaceAsync(string type, float height, float width, float length)
    {
        var learningSpace = new LearningSpace(type, height, width, length);
        await _repository.AddAsync(learningSpace);
        return learningSpace;
    }
}
