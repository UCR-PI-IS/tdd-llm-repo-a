namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying validation error details for failed requests.
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>
    /// Gets or sets the collection of validation error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new();
}
