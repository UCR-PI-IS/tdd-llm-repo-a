namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents a result of an operation that can succeed or fail.
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful service result.</returns>
    public static ServiceResult<object> Success()
    {
        return new ServiceResult<object> { IsSuccess = true };
    }

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed service result.</returns>
    public static ServiceResult<object> Failure(string errorMessage)
    {
        return new ServiceResult<object> { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
