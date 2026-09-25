namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for university-related service operations.
/// </summary>
public interface IUniversityService
{
    /// <summary>
    /// Adds a new university with the specified name and country.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country where the university is located.</param>
    /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure.</returns>
    Task<ServiceResult<bool>> AddUniversityAsync(string name, string country);
}
