namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space, such as a whiteboard or projector.
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
    /// Orientation of the learning component (North, South, East, West).
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
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, or orientation is invalid.</exception>
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
        ValidateNonNegative(width, nameof(width));
        ValidateNonNegative(height, nameof(height));
        ValidateNonNegative(depth, nameof(depth));
        ValidateNonNegative(x, nameof(x));
        ValidateNonNegative(y, nameof(y));
        ValidateNonNegative(z, nameof(z));
        ValidateOrientation(orientation);

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

    private static void ValidateNonNegative(float value, string paramName)
    {
        if (value < 0)
            throw new ArgumentException($"{paramName} cannot be negative.", paramName);
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Invalid orientation. Must be North, South, East, or West.", nameof(orientation));
    }
}
