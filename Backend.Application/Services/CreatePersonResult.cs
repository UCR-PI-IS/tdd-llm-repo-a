namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result of a person creation operation.
/// </summary>
public class CreatePersonResult
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message when the operation fails. Null on success.
    /// </summary>
    public string? ErrorMessage { get; }

    private CreatePersonResult(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CreatePersonResult Success() => new(true, null);

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public static CreatePersonResult Failure(string errorMessage) => new(false, errorMessage);
}
