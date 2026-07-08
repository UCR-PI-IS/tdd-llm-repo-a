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
    /// X coordinate of the component's position.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate of the component's position.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate of the component's position.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Constructor for the LearningComponent class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component</param>
    /// <param name="learningSpaceId">Identifier of the learning space</param>
    /// <param name="width">Width of the component in meters</param>
    /// <param name="height">Height of the component in meters</param>
    /// <param name="depth">Depth of the component in meters</param>
    /// <param name="x">X coordinate of the component's position</param>
    /// <param name="y">Y coordinate of the component's position</param>
    /// <param name="z">Z coordinate of the component's position</param>
    /// <param name="orientation">Orientation of the component</param>
    public LearningComponent(string componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        ValidateNonNegative(width, nameof(width), "Width cannot be negative.");
        ValidateNonNegative(height, nameof(height), "Height cannot be negative.");
        ValidateNonNegative(depth, nameof(depth), "Depth cannot be negative.");
        ValidateNonNegative(x, nameof(x), "X coordinate cannot be negative.");
        ValidateNonNegative(y, nameof(y), "Y coordinate cannot be negative.");
        ValidateNonNegative(z, nameof(z), "Z coordinate cannot be negative.");
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

    private static void ValidateNonNegative(float value, string paramName, string message)
    {
        if (value < 0)
            throw new ArgumentException(message, paramName);
    }

    private static void ValidateOrientation(string orientation)
    {
        if (orientation != "North" && orientation != "South" && orientation != "East" && orientation != "West")
            throw new ArgumentException("Orientation must be North, South, East, or West.", nameof(orientation));
    }
}
