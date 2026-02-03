namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for person operations.
/// </summary>
public interface IPersonService
{
    /// <summary>
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="firstName">First name of the person.</param>
    /// <param name="lastName">Last name of the person.</param>
    /// <param name="email">Email address of the person.</param>
    /// <param name="identityNumber">Identity number of the person.</param>
    /// <param name="birthDate">Birth date of the person.</param>
    /// <param name="phone">Phone number of the person (optional).</param>
    /// <returns>True if creation is successful, false otherwise.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a person with the same email or identity number already exists.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are missing or invalid.</exception>
    Task<bool> CreatePersonAsync(string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone);
}
