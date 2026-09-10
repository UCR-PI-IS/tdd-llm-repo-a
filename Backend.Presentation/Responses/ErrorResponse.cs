namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying an error message for failed requests.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP status code for the error.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message describing the failure.
    /// </summary>
    public required string Message { get; set; }
}
