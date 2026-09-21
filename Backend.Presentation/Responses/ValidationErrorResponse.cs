namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for validation errors.
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public List<string> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationErrorResponse"/> class.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationErrorResponse(List<string> errors)
    {
        Errors = errors;
    }
}
