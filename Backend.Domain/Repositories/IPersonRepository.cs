using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Repository interface for managing persons.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Adds a new person to the database.
    /// </summary>
    /// <param name="person">The person to add.</param>
    /// <returns>True if successful, false otherwise.</returns>
    Task<bool> AddAsync(Person person);

    /// <summary>
    /// Retrieves a person by email address.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The person if found, null otherwise.</returns>
    Task<Person?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves a person by identity number.
    /// </summary>
    /// <param name="identityNumber">The identity number to search for.</param>
    /// <returns>The person if found, null otherwise.</returns>
    Task<Person?> GetByIdentityNumberAsync(string identityNumber);
}
