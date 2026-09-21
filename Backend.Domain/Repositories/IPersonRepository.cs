using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for person repository operations in the data source.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Adds a new person to the data source.
    /// </summary>
    /// <param name="person">The person entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Person person);

    /// <summary>
    /// Checks if a person with the specified email already exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>True if a person with the email exists, otherwise false.</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Checks if a person with the specified identity number already exists.
    /// </summary>
    /// <param name="identityNumber">The identity number to check.</param>
    /// <returns>True if a person with the identity number exists, otherwise false.</returns>
    Task<bool> ExistsByIdentityNumberAsync(string identityNumber);
}
