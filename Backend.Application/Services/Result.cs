namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a service operation without a data payload.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; private set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful <see cref="Result"/>.</returns>
    public static Result Success() => new Result { IsSuccess = true };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A failed <see cref="Result"/>.</returns>
    public static Result Failure(string errorMessage) => new Result { IsSuccess = false, ErrorMessage = errorMessage };
}
