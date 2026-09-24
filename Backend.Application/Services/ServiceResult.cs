namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a service operation without data.
/// </summary>
public class ServiceResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static ServiceResult Success() => new ServiceResult { IsSuccess = true };

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    public static ServiceResult Failure(string message) => new ServiceResult { IsSuccess = false, ErrorMessage = message };
}

/// <summary>
/// Represents the result of a service operation.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public class ServiceResult<T>
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the data returned by the operation on success.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
