using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the person creation service.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person after checking for duplicates.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    Task<CreatePersonResult> CreatePersonAsync(Person person);
}
