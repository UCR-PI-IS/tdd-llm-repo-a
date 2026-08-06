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

    /// <summary>
    /// Valid orientations for a learning component.
    /// </summary>
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

    /// <summary>
    /// Validates that a value is not negative.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The parameter name for the exception.</param>
    /// <param name="paramDescription">The description of the parameter for the error message.</param>
    private static void ValidateNonNegative(float value, string paramName, string paramDescription)
    {
        if (value < 0)
            throw new ArgumentException($"{paramDescription} cannot be negative", paramName);
    }

    /// <summary>
    /// Constructor for the LearningComponent class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component</param>
    /// <param name="learningSpaceId">Identifier of the learning space</param>
    /// <param name="width">Width of the component</param>
    /// <param name="height">Height of the component</param>
    /// <param name="depth">Depth of the component</param>
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
        ValidateNonNegative(width, nameof(width), "Width");
        ValidateNonNegative(height, nameof(height), "Height");
        ValidateNonNegative(depth, nameof(depth), "Depth");
        ValidateNonNegative(x, nameof(x), "X coordinate");
        ValidateNonNegative(y, nameof(y), "Y coordinate");
        ValidateNonNegative(z, nameof(z), "Z coordinate");
        
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Invalid orientation. Must be North, South, East, or West", nameof(orientation));

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
