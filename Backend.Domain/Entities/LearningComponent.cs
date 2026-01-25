namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space.
/// A learning component is an interactive element like a whiteboard, projector, etc.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Gets the unique identifier for the component.
    /// </summary>
    public int ComponentId { get; private set; }

    /// <summary>
    /// Gets the identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; private set; }

    /// <summary>
    /// Gets the width of the component in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// Gets the height of the component in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Gets the depth of the component in meters.
    /// </summary>
    public float Depth { get; private set; }

    /// <summary>
    /// Gets the X coordinate of the component's position.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Gets the Y coordinate of the component's position.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Gets the Z coordinate of the component's position.
    /// </summary>
    public float Z { get; private set; }

    /// <summary>
    /// Gets the orientation of the component (e.g., North, South, East, West).
    /// </summary>
    public string Orientation { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="componentId">The unique identifier for the component.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <param name="width">The width of the component.</param>
    /// <param name="height">The height of the component.</param>
    /// <param name="depth">The depth of the component.</param>
    /// <param name="x">The X coordinate position.</param>
    /// <param name="y">The Y coordinate position.</param>
    /// <param name="z">The Z coordinate position.</param>
    /// <param name="orientation">The orientation of the component.</param>
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
