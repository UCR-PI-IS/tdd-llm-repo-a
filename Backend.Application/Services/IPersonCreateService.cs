using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that handles person creation.
/// </summary>
public interface IPersonCreateService
{
    /// <summary>
    /// Creates a new person after validating for duplicates.
    /// </summary>
    /// <param name="person">The person to create.</param>
    /// <returns>A result indicating success or failure of the creation.</returns>
    Task<CreatePersonResult> CreatePersonAsync(Person person);
}
