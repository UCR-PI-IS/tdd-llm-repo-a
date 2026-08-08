using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the component.</param>
/// <param name="LearningSpaceId">The identifier of the learning space.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Depth">Depth in meters.</param>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
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
    public static LearningComponentDto FromDomain(LearningComponent c) =>
        new(c.ComponentId, c.LearningSpaceId, c.Width, c.Height, c.Depth, c.X, c.Y, c.Z, c.Orientation);
}
