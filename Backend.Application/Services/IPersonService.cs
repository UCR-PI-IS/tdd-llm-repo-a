using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for person management services.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>An operation result containing the created person or an error message.</returns>
    Task<OperationResult<Person>> CreatePersonAsync(Person person);
}
