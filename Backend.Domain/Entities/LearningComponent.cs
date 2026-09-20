namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents an interactive learning component within a learning space,
/// such as a whiteboard or projector.
/// </summary>
public class LearningComponent
{
    private static readonly HashSet<string> ValidOrientations = new()
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
    public float Width { get; protected set; }

    /// <summary>
    /// Height of the learning component in meters.
    /// </summary>
    public float Height { get; protected set; }

    /// <summary>
    /// Depth of the learning component in meters.
    /// </summary>
    public float Depth { get; protected set; }

    /// <summary>
    /// X coordinate of the learning component position within the learning space.
    /// </summary>
    public float X { get; protected set; }

    /// <summary>
    /// Y coordinate of the learning component position within the learning space.
    /// </summary>
    public float Y { get; protected set; }

    /// <summary>
    /// Z coordinate of the learning component position within the learning space.
    /// </summary>
    public float Z { get; protected set; }

    /// <summary>
    /// Orientation of the learning component (North, South, East, or West).
    /// </summary>
    public string Orientation { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// Auto-generates a unique component ID.
    /// </summary>
    /// <param name="learningSpaceId">Identifier of the learning space this component belongs to.</param>
    /// <param name="width">Width of the component in meters. Must be positive.</param>
    /// <param name="height">Height of the component in meters. Must be positive.</param>
    /// <param name="depth">Depth of the component in meters. Must be positive.</param>
    /// <param name="x">X coordinate position. Must be non-negative.</param>
    /// <param name="y">Y coordinate position. Must be non-negative.</param>
    /// <param name="z">Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">Orientation of the component. Must be North, South, East, or West.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension is not positive, any coordinate is negative, or orientation is invalid.</exception>
    public LearningComponent(
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation)
    {
        ThrowIfNotPositive(width, nameof(width));
        ThrowIfNotPositive(height, nameof(height));
        ThrowIfNotPositive(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
        ValidateOrientation(orientation);

        ComponentId = Guid.NewGuid().ToString("N");
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the learning component.</param>
    /// <param name="learningSpaceId">Identifier of the learning space this component belongs to.</param>
    /// <param name="width">Width of the component in meters. Must be non-negative.</param>
    /// <param name="height">Height of the component in meters. Must be non-negative.</param>
    /// <param name="depth">Depth of the component in meters. Must be non-negative.</param>
    /// <param name="x">X coordinate position. Must be non-negative.</param>
    /// <param name="y">Y coordinate position. Must be non-negative.</param>
    /// <param name="z">Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">Orientation of the component. Must be North, South, East, or West.</param>
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
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
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

    private static void ThrowIfNegative(float value, string paramName)
    {
        if (value < 0f)
            throw new ArgumentException($"{Capitalize(paramName)} cannot be negative.", paramName);
    }

    private static void ThrowIfNotPositive(float value, string paramName)
    {
        if (value <= 0f)
            throw new ArgumentException($"{Capitalize(paramName)} must be positive.", paramName);
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid Orientation '{orientation}'. Must be one of: North, South, East, West.",
                nameof(orientation));
    }

    private static string Capitalize(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        return char.ToUpper(value[0]) + value[1..];
    }
}
