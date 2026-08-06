namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

/// <summary>
/// Represents an error response returned by the API.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Gets or sets the error code (optional).
    /// </summary>
    public string? Code { get; set; }
}
