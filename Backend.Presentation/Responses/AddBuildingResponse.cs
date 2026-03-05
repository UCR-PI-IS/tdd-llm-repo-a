namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Represents the response for an add building operation.
/// </summary>
public class AddBuildingResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
