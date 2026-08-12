namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space,
/// such as a whiteboard or projector, with physical dimensions,
/// position coordinates, and orientation.
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
    /// Width of the learning component in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the learning component in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Depth of the learning component in meters.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// X coordinate of the learning component position.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate of the learning component position.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate of the learning component position.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the learning component (North, South, East, or West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component.</param>
    /// <param name="learningSpaceId">Identifier of the parent learning space.</param>
    /// <param name="width">Width in meters (must be non-negative).</param>
    /// <param name="height">Height in meters (must be non-negative).</param>
    /// <param name="depth">Depth in meters (must be non-negative).</param>
    /// <param name="x">X coordinate (must be non-negative).</param>
    /// <param name="y">Y coordinate (must be non-negative).</param>
    /// <param name="z">Z coordinate (must be non-negative).</param>
    /// <param name="orientation">Orientation (North, South, East, or West).</param>
    /// <exception cref="ArgumentException">
    /// Thrown when any dimension or coordinate is negative, or when orientation is invalid.
    /// </exception>
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
        ThrowIfNegative(width, nameof(width), "Width cannot be negative.");
        ThrowIfNegative(height, nameof(height), "Height cannot be negative.");
        ThrowIfNegative(depth, nameof(depth), "Depth cannot be negative.");
        ThrowIfNegative(x, nameof(x), "X coordinate cannot be negative.");
        ThrowIfNegative(y, nameof(y), "Y coordinate cannot be negative.");
        ThrowIfNegative(z, nameof(z), "Z coordinate cannot be negative.");
        ThrowIfInvalidOrientation(orientation);

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

    private static void ThrowIfNegative(float value, string paramName, string message)
    {
        if (value < 0)
            throw new ArgumentException(message, paramName);
    }

    private static void ThrowIfInvalidOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Valid values are: North, South, East, West.",
                nameof(orientation));
    }
}
