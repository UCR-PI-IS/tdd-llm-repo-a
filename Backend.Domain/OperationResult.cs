namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public class OperationResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the value returned by the operation on success.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Creates a successful operation result with the specified value.
    /// </summary>
    /// <param name="value">The value to return.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult<T> Success(T value)
    {
        return new OperationResult<T> { IsSuccess = true, Value = value };
    }

    /// <summary>
    /// Creates a failed operation result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Failure(string errorMessage)
    {
        return new OperationResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
