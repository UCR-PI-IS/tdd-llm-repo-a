namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component (e.g., whiteboard, projector) within a learning space.
/// </summary>
public class LearningComponent
{
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

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
    /// Orientation of the component (North, South, East, or West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Constructor for the LearningComponent class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component</param>
    /// <param name="learningSpaceId">Identifier of the learning space</param>
    /// <param name="width">Width of the component in meters (must be >= 0)</param>
    /// <param name="height">Height of the component in meters (must be >= 0)</param>
    /// <param name="depth">Depth of the component in meters (must be >= 0)</param>
    /// <param name="x">X coordinate position (must be >= 0)</param>
    /// <param name="y">Y coordinate position (must be >= 0)</param>
    /// <param name="z">Z coordinate position (must be >= 0)</param>
    /// <param name="orientation">Orientation (North, South, East, or West)</param>
    public LearningComponent(string componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
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

    private static void ThrowIfNegative(float value, string paramName)
    {
        if (value < 0)
            throw new ArgumentException("Value cannot be negative.", paramName);
    }

    private static void ThrowIfInvalidOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be North, South, East, or West.", nameof(orientation));
    }
}
