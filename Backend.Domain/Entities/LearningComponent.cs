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
    /// Orientation of the component within the learning space.
    /// </summary>
    public Enum Orientation { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the component.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width of the component.</param>
    /// <param name="height">Height of the component.</param>
    /// <param name="depth">Depth of the component.</param>
    /// <param name="x">X coordinate position.</param>
    /// <param name="y">Y coordinate position.</param>
    /// <param name="z">Z coordinate position.</param>
    /// <param name="orientation">Orientation of the component.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public LearningComponent(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        Enum orientation)
    {
        if (width < 0)
            throw new ArgumentException("Width cannot be negative.", nameof(width));
        
        if (height < 0)
            throw new ArgumentException("Height cannot be negative.", nameof(height));
        
        if (depth < 0)
            throw new ArgumentException("Depth cannot be negative.", nameof(depth));
        
        if (x < 0)
            throw new ArgumentException("X coordinate cannot be negative.", nameof(x));
        
        if (y < 0)
            throw new ArgumentException("Y coordinate cannot be negative.", nameof(y));
        
        if (z < 0)
            throw new ArgumentException("Z coordinate cannot be negative.", nameof(z));
        
        if (orientation == null)
            throw new ArgumentException("Orientation cannot be null.", nameof(orientation));

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
