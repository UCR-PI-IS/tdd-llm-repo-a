namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result object returned after creating a learning component.
/// </summary>
public class CreateComponentResult
{
    /// <summary>
    /// The identifier assigned to the created component.
    /// </summary>
    public string ComponentId { get; set; } = string.Empty;

    /// <summary>
    /// A human-readable message describing the result.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
