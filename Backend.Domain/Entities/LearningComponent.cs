namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component (e.g., whiteboard, projector) within a learning space.
/// </summary>
public class LearningComponent
{
    private static readonly HashSet<string> ValidOrientations = new(StringComparer.OrdinalIgnoreCase)
    {
        "North", "South", "East", "West"
    };

    /// <summary>
    /// Unique identifier for the learning component.
    /// </summary>
    public string ComponentId { get; }

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; }

    /// <summary>
    /// Width of the component in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the component in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Depth of the component in meters.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// X coordinate position within the learning space.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate position within the learning space.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate position within the learning space.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width in meters (must be >= 0).</param>
    /// <param name="height">Height in meters (must be >= 0).</param>
    /// <param name="depth">Depth in meters (must be >= 0).</param>
    /// <param name="x">X coordinate (must be >= 0).</param>
    /// <param name="y">Y coordinate (must be >= 0).</param>
    /// <param name="z">Z coordinate (must be >= 0).</param>
    /// <param name="orientation">Orientation (must be North, South, East, or West).</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public LearningComponent(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation)
    {
        GuardNonNegative(width, nameof(width), "Width cannot be negative.");
        GuardNonNegative(height, nameof(height), "Height cannot be negative.");
        GuardNonNegative(depth, nameof(depth), "Depth cannot be negative.");
        GuardNonNegative(x, nameof(x), "X coordinate cannot be negative.");
        GuardNonNegative(y, nameof(y), "Y coordinate cannot be negative.");
        GuardNonNegative(z, nameof(z), "Z coordinate cannot be negative.");
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be one of: North, South, East, West.", nameof(orientation));

        ComponentId = componentId;
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

    private static void GuardNonNegative(float value, string paramName, string message)
    {
        if (value < 0)
            throw new ArgumentException(message, paramName);
    }
}