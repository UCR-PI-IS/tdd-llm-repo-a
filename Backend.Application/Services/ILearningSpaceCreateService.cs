using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service contract for creating learning spaces.
/// </summary>
public interface ILearningSpaceCreateService
{
    /// <summary>
    /// Creates a new learning space with the specified parameters.
    /// </summary>
    /// <param name="id">Unique identifier for the learning space</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    /// <returns>The created learning space entity</returns>
    Task<LearningSpace> CreateLearningSpaceAsync(string id, string type, float height, float width, float length);
}
