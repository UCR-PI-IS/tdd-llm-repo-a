using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space.
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
    /// X coordinate of the component.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate of the component.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate of the component.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component.
    /// </summary>
    public Orientation Orientation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component</param>
    /// <param name="learningSpaceId">Identifier of the learning space</param>
    /// <param name="width">Width of the component</param>
    /// <param name="height">Height of the component</param>
    /// <param name="depth">Depth of the component</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="orientation">Orientation of the component</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
    public LearningComponent(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        Orientation orientation)
    {
        ValidateNonNegative(width, nameof(width), "Width cannot be negative");
        ValidateNonNegative(height, nameof(height), "Height cannot be negative");
        ValidateNonNegative(depth, nameof(depth), "Depth cannot be negative");
        ValidateNonNegative(x, nameof(x), "X coordinate cannot be negative");
        ValidateNonNegative(y, nameof(y), "Y coordinate cannot be negative");
        ValidateNonNegative(z, nameof(z), "Z coordinate cannot be negative");
        if (!Enum.IsDefined(typeof(Orientation), orientation))
            throw new ArgumentException("Invalid orientation value", nameof(orientation));

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
}
