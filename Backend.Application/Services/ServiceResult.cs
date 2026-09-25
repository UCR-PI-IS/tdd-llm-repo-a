namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a service operation.
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
}

/// <summary>
/// Represents the result of a service operation with data.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public class ServiceResult<T> : ServiceResult
{
    /// <summary>
    /// Gets or sets the data returned by the operation on success.
    /// </summary>
    public T? Data { get; set; }
}
