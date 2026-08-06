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
    /// <exception cref="ArgumentException">Thrown when any validation fails</exception>
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
            throw new ArgumentException("Width cannot be negative", "width");
        
        if (height < 0)
            throw new ArgumentException("Height cannot be negative", "height");
        
        if (depth < 0)
            throw new ArgumentException("Depth cannot be negative", "depth");
    }

    private static void ValidateCoordinates(float x, float y, float z)
    {
        if (x < 0)
            throw new ArgumentException("X coordinate cannot be negative", "x");
        
        if (y < 0)
            throw new ArgumentException("Y coordinate cannot be negative", "y");
        
        if (z < 0)
            throw new ArgumentException("Z coordinate cannot be negative", "z");
    }

    private static void ValidateOrientation(string orientation)
    {
        if (orientation != "North" && orientation != "South" && orientation != "East" && orientation != "West")
            throw new ArgumentException("Orientation must be North, South, East, or West", "orientation");
    }
}
