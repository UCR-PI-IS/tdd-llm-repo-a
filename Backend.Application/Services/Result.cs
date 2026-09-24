namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a service operation.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    private Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A success result.</returns>
    public static Result Success()
    {
        return new Result(true, null);
    }

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failure result.</returns>
    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }
}
