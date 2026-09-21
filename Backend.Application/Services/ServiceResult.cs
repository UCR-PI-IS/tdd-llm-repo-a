namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a service operation.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the result value if the operation was successful.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; }

    private ServiceResult(bool isSuccess, T? value, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The result value.</param>
    /// <returns>A successful service result.</returns>
    public static ServiceResult<T> Success(T value)
    {
        return new ServiceResult<T>(true, value, null);
    }

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed service result.</returns>
    public static ServiceResult<T> Failure(string errorMessage)
    {
        return new ServiceResult<T>(false, default, errorMessage);
    }
}
