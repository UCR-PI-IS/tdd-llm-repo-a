using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that creates persons.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    Task<CreatePersonResult> CreatePersonAsync(Person person);
}
