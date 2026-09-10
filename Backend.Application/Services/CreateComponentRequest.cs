namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object for creating a new learning component.
/// </summary>
public class CreateComponentRequest
{
    /// <summary>
    /// Optional explicit component ID. If null or empty, an ID will be auto-generated.
    /// </summary>
    public string? ComponentId { get; set; }

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public required string LearningSpaceId { get; set; }

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
    /// X coordinate position.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate position.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate position.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Orientation of the component (North, South, East, or West).
    /// </summary>
    public required string Orientation { get; set; }
}
