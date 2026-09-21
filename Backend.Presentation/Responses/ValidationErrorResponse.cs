namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying validation errors for failed requests.
/// </summary>
public record class ValidationErrorResponse
{
    /// <summary>
    /// Gets the list of validation errors.
    /// </summary>
    public List<string> Errors { get; init; } = new List<string>();

    /// <summary>
    /// Creates a validation error response from a list of error messages.
    /// </summary>
    /// <param name="errors">The list of error messages.</param>
    /// <returns>A validation error response.</returns>
    public static ValidationErrorResponse FromErrors(List<string> errors)
    {
        return new ValidationErrorResponse { Errors = errors };
    }
}
