namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result object returned after creating a learning component.
/// </summary>
public class CreateComponentResult
{
    /// <summary>
    /// The component ID (auto-generated or explicit).
    /// </summary>
    public required string ComponentId { get; set; }

    /// <summary>
    /// A message describing the outcome of the creation.
    /// </summary>
    public required string Message { get; set; }
}
