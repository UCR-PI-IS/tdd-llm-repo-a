namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space of the theme park UCR.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Unique identifier for the learning component.
    /// </summary>
    public string ComponentId { get; }

    /// <summary>
    /// Identifier of the learning space where this component is located.
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
    /// X coordinate position in the learning space.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate position in the learning space.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate position in the learning space.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; }

    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

    /// <summary>
    /// Validates that a value is not negative.
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <param name="paramName">The parameter name for the exception</param>
    /// <param name="message">The error message</param>
    private static void ValidateNonNegative(float value, string paramName, string message)
    {
        if (value < 0)
            throw new ArgumentException(message, paramName);
    }

    /// <summary>
    /// Validates that an orientation value is valid.
    /// </summary>
    /// <param name="orientation">The orientation to validate</param>
    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be North, South, East, or West.", nameof(orientation));
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
    /// <param name="orientation">Orientation of the component (North, South, East, West)</param>
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
}
