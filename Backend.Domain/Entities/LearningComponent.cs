namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Gets or sets the unique identifier for the learning component.
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated learning space.
    /// </summary>
    public string LearningSpaceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the width of the component.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the component.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the depth of the component.
    /// </summary>
    public double Depth { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate position.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate position.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate position.
    /// </summary>
    public double Z { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the component.
    /// </summary>
    public double Orientation { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated learning space.
    /// </summary>
    public LearningSpace? LearningSpace { get; set; }
}
