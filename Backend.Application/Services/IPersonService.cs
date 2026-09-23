using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that manages person creation.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person after checking for duplicates.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A result indicating success or failure of the creation.</returns>
    Task<CreatePersonResult> CreatePersonAsync(Person person);
}
