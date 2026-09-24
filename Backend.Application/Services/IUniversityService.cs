using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for university service operations.
/// </summary>
public interface IUniversityService
{
    /// <summary>
    /// Adds a new university to the system.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country of the university.</param>
    /// <returns>A service result indicating success or failure.</returns>
    Task<ServiceResult<object>> AddUniversityAsync(string name, string country);
}
