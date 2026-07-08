using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Entities;

/// <summary>
/// Entity representing a learning component in the database.
/// </summary>
public class LearningComponentEntity
{
    /// <summary>
    /// Unique identifier for the component.
    /// </summary>
    public string ComponentId { get; set; } = null!;

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; set; } = null!;

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
    /// X coordinate of the component.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate of the component.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate of the component.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Orientation of the component.
    /// </summary>
    public Orientation Orientation { get; set; }

    /// <summary>
    /// Converts this entity to a domain model.
    /// </summary>
    /// <returns>A <see cref="LearningComponent"/> domain model.</returns>
    public LearningComponent ToDomain() => new(
        ComponentId,
        LearningSpaceId,
        Width,
        Height,
        Depth,
        X,
        Y,
        Z,
        Orientation);
}
