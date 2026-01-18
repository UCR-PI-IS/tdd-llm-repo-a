namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Standard error response for API failures.
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; }

    public ErrorResponse(string message)
    {
        Message = message;
    }
}
