using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for person service operations.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A service result containing the created person or an error message.</returns>
    Task<ServiceResult<Person>> CreatePersonAsync(Person person);
}
