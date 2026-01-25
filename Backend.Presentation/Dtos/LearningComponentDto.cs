namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

/// <summary>
/// Data Transfer Object for LearningComponent.
/// </summary>
public class LearningComponentDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the component.
    /// </summary>
    public int ComponentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the width of the component in meters.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the component in meters.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Gets or sets the depth of the component in meters.
    /// </summary>
    public float Depth { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate of the component's position.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the component's position.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the component's position.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the component.
    /// </summary>
    public string Orientation { get; set; } = string.Empty;
}
