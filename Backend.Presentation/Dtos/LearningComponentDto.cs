using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the learning component.</param>
/// <param name="LearningSpaceId">The identifier of the learning space this component belongs to.</param>
/// <param name="Width">Width of the component in meters.</param>
/// <param name="Height">Height of the component in meters.</param>
/// <param name="Depth">Depth of the component in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the component.</param>
public record class LearningComponentDto(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation)
{
    /// <summary>
    /// Creates a <see cref="LearningComponentDto"/> from a domain <see cref="LearningComponent"/> entity.
    /// </summary>
    public static LearningComponentDto FromDomain(LearningComponent c) => new(
        c.ComponentId, c.LearningSpaceId, c.Width, c.Height, c.Depth, c.X, c.Y, c.Z, c.Orientation);
}
