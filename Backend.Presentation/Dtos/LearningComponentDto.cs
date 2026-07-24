using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the learning component.</param>
/// <param name="LearningSpaceId">The identifier of the owning learning space.</param>
/// <param name="Width">Width of the component.</param>
/// <param name="Height">Height of the component.</param>
/// <param name="Depth">Depth of the component.</param>
/// <param name="X">X coordinate within the learning space.</param>
/// <param name="Y">Y coordinate within the learning space.</param>
/// <param name="Z">Z coordinate within the learning space.</param>
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
    /// Maps a domain entity to its presentation DTO.
    /// </summary>
    /// <param name="component">The domain learning component.</param>
    /// <returns>The corresponding DTO.</returns>
    public static LearningComponentDto FromEntity(LearningComponent component)
    {
        return new LearningComponentDto(
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
}
