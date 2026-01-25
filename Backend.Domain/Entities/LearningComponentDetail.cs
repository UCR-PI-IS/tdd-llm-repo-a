namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents detailed information about a learning component.
/// </summary>
public class LearningComponentDetail
{
    /// <summary>
    /// Gets or sets the unique identifier for the learning component detail.
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated learning component.
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Gets or sets the description of the learning component.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets additional metadata for the learning component.
    /// </summary>
    public string Metadata { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the navigation property to the associated learning component.
    /// </summary>
    public LearningComponent? Component { get; set; }
}
