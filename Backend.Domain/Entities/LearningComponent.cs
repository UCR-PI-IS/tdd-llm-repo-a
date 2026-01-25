namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Unique identifier for the learning component.
    /// </summary>
    public int ComponentId { get; }

    /// <summary>
    /// Identifier of the learning space containing this component.
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
    /// Orientation of the component (e.g., North, South, East, West).
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
    /// <param name="x">X coordinate position</param>
    /// <param name="y">Y coordinate position</param>
    /// <param name="z">Z coordinate position</param>
    /// <param name="orientation">Orientation of the component</param>
    public LearningComponent(
        int componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation)
    {
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
