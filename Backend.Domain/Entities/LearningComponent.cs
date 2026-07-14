using System.Linq;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space of the theme park UCR.
/// </summary>
public class LearningComponent
{
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };
    /// <summary>
    /// Unique identifier for the learning component.
    /// </summary>
    public string ComponentId { get; private set; } = string.Empty;

    /// <summary>
    /// Identifier of the learning space where this component is located.
    /// </summary>
    public string LearningSpaceId { get; private set; } = string.Empty;

    /// <summary>
    /// Width of the component in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// Height of the component in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Depth of the component in meters.
    /// </summary>
    public float Depth { get; private set; }

    /// <summary>
    /// X coordinate position in the learning space.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Y coordinate position in the learning space.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Z coordinate position in the learning space.
    /// </summary>
    public float Z { get; private set; }

    /// <summary>
    /// Orientation of the component (North, South, East, West).
    /// </summary>
    public string Orientation { get; private set; } = string.Empty;

    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private LearningComponent()
    {
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
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid</exception>
    public LearningComponent(string componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        ValidateDimensions(width, height, depth);
        ValidateCoordinates(x, y, z);
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

    private static void ValidateDimensions(float width, float height, float depth)
    {
        if (width < 0)
            throw new ArgumentException("Width cannot be negative", nameof(width));
        
        if (height < 0)
            throw new ArgumentException("Height cannot be negative", nameof(height));
        
        if (depth < 0)
            throw new ArgumentException("Depth cannot be negative", nameof(depth));
    }

    private static void ValidateCoordinates(float x, float y, float z)
    {
        if (x < 0)
            throw new ArgumentException("X coordinate cannot be negative", nameof(x));
        
        if (y < 0)
            throw new ArgumentException("Y coordinate cannot be negative", nameof(y));
        
        if (z < 0)
            throw new ArgumentException("Z coordinate cannot be negative", nameof(z));
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be North, South, East, or West", nameof(orientation));
    }
}
