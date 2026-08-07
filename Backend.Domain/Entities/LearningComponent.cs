namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public String ComponentId { get; }

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public String LearningSpaceId { get; }

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
    public String Orientation { get; }

    private static readonly HashSet<String> ValidOrientations = new HashSet<String> { "North", "South", "East", "West" };

    /// <summary>
    /// Validates that a value is not negative.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="errorMessage">The error message if validation fails.</param>
    private static void ValidateNonNegative(float value, string paramName, string errorMessage)
    {
        if (value < 0)
            throw new ArgumentException(errorMessage, paramName);
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
    public LearningComponent(String componentId, String learningSpaceId, float width, float height, float depth, float x, float y, float z, String orientation)
    {
        ValidateNonNegative(width, "width", "Width cannot be negative.");
        ValidateNonNegative(height, "height", "Height cannot be negative.");
        ValidateNonNegative(depth, "depth", "Depth cannot be negative.");
        ValidateNonNegative(x, "x", "X coordinate cannot be negative.");
        ValidateNonNegative(y, "y", "Y coordinate cannot be negative.");
        ValidateNonNegative(z, "z", "Z coordinate cannot be negative.");
        
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be North, South, East, or West.", "orientation");

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
