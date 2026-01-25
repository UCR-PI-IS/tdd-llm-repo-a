namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component within a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public int ComponentId { get; set; }

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; set; }

    /// <summary>
    /// Width of the component.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// Height of the component.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Depth of the component.
    /// </summary>
    public float Depth { get; set; }

    /// <summary>
    /// X coordinate position.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate position.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate position.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Orientation of the component.
    /// </summary>
    public string Orientation { get; set; }

    /// <summary>
    /// Constructor for LearningComponent.
    /// </summary>
    public LearningComponent(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
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
