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
    /// Identifier of the learning space that owns this component.
    /// </summary>
    public string LearningSpaceId { get; }

    /// <summary>
    /// Width of the component.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the component.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Depth of the component.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// X coordinate of the component within the learning space.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate of the component within the learning space.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate of the component within the learning space.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component (North, South, East, or West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component.</param>
    /// <param name="learningSpaceId">Identifier of the owning learning space.</param>
    /// <param name="width">Width of the component (must be non-negative).</param>
    /// <param name="height">Height of the component (must be non-negative).</param>
    /// <param name="depth">Depth of the component (must be non-negative).</param>
    /// <param name="x">X coordinate (must be non-negative).</param>
    /// <param name="y">Y coordinate (must be non-negative).</param>
    /// <param name="z">Z coordinate (must be non-negative).</param>
    /// <param name="orientation">Orientation (North, South, East, or West).</param>
    /// <exception cref="ArgumentException">Thrown when a dimension, coordinate, or orientation is invalid.</exception>
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
        Width = EnsureNonNegative(width, nameof(width), "Width cannot be negative.");
        Height = EnsureNonNegative(height, nameof(height), "Height cannot be negative.");
        Depth = EnsureNonNegative(depth, nameof(depth), "Depth cannot be negative.");
        X = EnsureNonNegative(x, nameof(x), "X coordinate cannot be negative.");
        Y = EnsureNonNegative(y, nameof(y), "Y coordinate cannot be negative.");
        Z = EnsureNonNegative(z, nameof(z), "Z coordinate cannot be negative.");
        Orientation = EnsureValidOrientation(orientation);

        ComponentId = componentId;
        LearningSpaceId = learningSpaceId;
    }

    private static float EnsureNonNegative(float value, string paramName, string message)
    {
        if (value < 0)
        {
            throw new ArgumentException(message, paramName);
        }

        return value;
    }

    private static string EnsureValidOrientation(string orientation)
    {
        if (string.IsNullOrWhiteSpace(orientation) || !ValidOrientations.Contains(orientation))
        {
            throw new ArgumentException(
                "Orientation must be one of: North, South, East, West.",
                nameof(orientation));
        }

        return orientation;
    }
}
