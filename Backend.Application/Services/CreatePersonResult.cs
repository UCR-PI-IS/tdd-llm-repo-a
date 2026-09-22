namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a person creation operation.
/// </summary>
public class CreatePersonResult
{
    /// <summary>
    /// Gets a value indicating whether the creation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the creation failed.
    /// </summary>
    public string ErrorMessage { get; }

    private CreatePersonResult(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static CreatePersonResult Success() => new(true, string.Empty);

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    public static CreatePersonResult Failure(string errorMessage) => new(false, errorMessage);
}
