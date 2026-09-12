namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Standard error response payload for API failures.
/// </summary>
public record class ErrorResponse(string Message)
{
    /// <summary>
    /// Creates an error response from an exception.
    /// </summary>
    public ErrorResponse(System.Exception ex) : this(ex.Message)
    {
    }
}
