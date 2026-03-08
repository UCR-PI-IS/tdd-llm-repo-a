namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// The error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new Result(true);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result Failure(string errorMessage) => new Result(false, errorMessage);
}
