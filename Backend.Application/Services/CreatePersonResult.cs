using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a person creation operation.
/// </summary>
public class CreatePersonResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; }

    /// <summary>
    /// Gets the created person if the operation succeeded; otherwise, null.
    /// </summary>
    public Person? Person { get; }

    private CreatePersonResult(bool isSuccess, string? errorMessage, Person? person)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Person = person;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="person">The created person.</param>
    /// <returns>A successful <see cref="CreatePersonResult"/>.</returns>
    public static CreatePersonResult Success(Person person)
    {
        return new CreatePersonResult(true, null, person);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="CreatePersonResult"/>.</returns>
    public static CreatePersonResult Failure(string errorMessage)
    {
        return new CreatePersonResult(false, errorMessage, null);
    }
}
