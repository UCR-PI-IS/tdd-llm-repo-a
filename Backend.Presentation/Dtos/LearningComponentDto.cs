namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

/// <summary>
/// Data transfer object representing a learning component.
/// </summary>
public class LearningComponentDto
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public string ComponentId { get; set; } = null!;

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; set; } = null!;

    /// <summary>
    /// Width of the component in meters.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Height of the component in meters.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Depth of the component in meters.
    /// </summary>
    public float Depth { get; set; }

    /// <summary>
    /// X coordinate position in the learning space.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate position in the learning space.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate position in the learning space.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; set; } = null!;
}
