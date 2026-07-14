namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Represents an error response returned by the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// </summary>
    public ErrorResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ErrorResponse(string message)
    {
        Message = message;
    }
}
