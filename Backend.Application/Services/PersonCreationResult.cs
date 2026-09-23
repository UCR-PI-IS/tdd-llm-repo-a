namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents the result of a person creation operation.
/// </summary>
public class PersonCreationResult
{
    /// <summary>
    /// Indicates whether the creation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Error message if the creation failed.
    /// </summary>
    public string ErrorMessage { get; }

    private PersonCreationResult(bool isSuccess, string errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful creation result.
    /// </summary>
    /// <returns>A success result.</returns>
    public static PersonCreationResult Success()
    {
        return new PersonCreationResult(true, string.Empty);
    }

    /// <summary>
    /// Creates a failed creation result.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failure result.</returns>
    public static PersonCreationResult Failure(string errorMessage)
    {
        return new PersonCreationResult(false, errorMessage);
    }
}
