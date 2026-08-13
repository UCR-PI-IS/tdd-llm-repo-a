namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space.
/// </summary>
public class LearningComponent
{
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
    /// X coordinate position of the component.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate position of the component.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate position of the component.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Valid orientation values for learning components.
    /// </summary>
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width of the component in meters.</param>
    /// <param name="height">Height of the component in meters.</param>
    /// <param name="depth">Depth of the component in meters.</param>
    /// <param name="x">X coordinate position.</param>
    /// <param name="y">Y coordinate position.</param>
    /// <param name="z">Z coordinate position.</param>
    /// <param name="orientation">Orientation of the component.</param>
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
            throw new ArgumentException($"Invalid orientation. Must be one of: {string.Join(", ", ValidOrientations)}", nameof(orientation));
    }
}
