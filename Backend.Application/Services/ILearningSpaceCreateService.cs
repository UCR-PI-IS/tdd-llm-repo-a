using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for creating learning spaces.
/// </summary>
public interface ILearningSpaceCreateService
{
    /// <summary>
    /// Creates a new learning space with the specified parameters.
    /// </summary>
    /// <param name="type">The type of the learning space.</param>
    /// <param name="height">The height of the learning space in meters.</param>
    /// <param name="width">The width of the learning space in meters.</param>
    /// <param name="length">The length of the learning space in meters.</param>
    /// <returns>The created learning space entity.</returns>
    Task<LearningSpace> CreateLearningSpaceAsync(string type, float height, float width, float length);
}
