namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a person creation operation.
/// </summary>
public class CreatePersonResult
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The error message if the operation failed; empty if it succeeded.
    /// </summary>
    public string ErrorMessage { get; }

    private CreatePersonResult(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CreatePersonResult Success() => new(true, string.Empty);

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    public static CreatePersonResult Failure(string errorMessage) => new(false, errorMessage);
}
