using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the person service that handles person creation and management.
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

/// <summary>
/// Represents the result of an operation that may succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the value returned by the operation on success.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Gets or sets the error message when the operation fails.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
