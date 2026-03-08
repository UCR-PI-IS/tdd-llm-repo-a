namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response for whiteboard creation operation.
/// </summary>
public class CreateWhiteboardResponse
{
    /// <summary>
    /// Indicates if the creation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Error message if the creation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
