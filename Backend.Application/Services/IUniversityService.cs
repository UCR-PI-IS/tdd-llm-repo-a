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
    /// <param name="country">The country where the university is located.</param>
    /// <returns>A result indicating success or failure of the operation.</returns>
    Task<Result> AddUniversityAsync(string name, string country);
}
