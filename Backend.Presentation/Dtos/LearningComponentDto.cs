using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

/// <summary>
/// Data transfer object for a learning component.
/// </summary>
public class LearningComponentDto
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
    /// Initializes a new instance of the <see cref="LearningComponentDto"/> class.
    /// </summary>
    public LearningComponentDto(
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

    /// <summary>
    /// Creates a <see cref="LearningComponentDto"/> from a domain entity.
    /// </summary>
    public static LearningComponentDto FromDomain(LearningComponent component) => new(
        component.ComponentId,
        component.LearningSpaceId,
        component.Width,
        component.Height,
        component.Depth,
        component.X,
        component.Y,
        component.Z,
        component.Orientation);
}
