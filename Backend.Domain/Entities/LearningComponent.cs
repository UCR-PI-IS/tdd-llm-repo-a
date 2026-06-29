namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Unique identifier for the component.
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

    private static readonly HashSet<string> ValidOrientations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "North", "South", "East", "West"
    };

    private static void GuardAgainstNegative(float value, string paramName, string displayName)
    {
        if (value < 0)
            throw new ArgumentException($"{displayName} cannot be negative.", paramName);
    }

    /// <summary>
    /// Constructor for the LearningComponent class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component</param>
    /// <param name="learningSpaceId">Identifier of the learning space</param>
    /// <param name="width">Width of the component in meters</param>
    /// <param name="height">Height of the component in meters</param>
    /// <param name="depth">Depth of the component in meters</param>
    /// <param name="x">X coordinate position</param>
    /// <param name="y">Y coordinate position</param>
    /// <param name="z">Z coordinate position</param>
    /// <param name="orientation">Orientation of the component</param>
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
        GuardAgainstNegative(width, nameof(width), "Width");
        GuardAgainstNegative(height, nameof(height), "Height");
        GuardAgainstNegative(depth, nameof(depth), "Depth");
        GuardAgainstNegative(x, nameof(x), "X coordinate");
        GuardAgainstNegative(y, nameof(y), "Y coordinate");
        GuardAgainstNegative(z, nameof(z), "Z coordinate");

        if (string.IsNullOrWhiteSpace(orientation) || !ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be North, South, East, or West.", nameof(orientation));

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
}
