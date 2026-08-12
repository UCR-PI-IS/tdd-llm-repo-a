namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the learning component.</param>
/// <param name="LearningSpaceId">The identifier of the parent learning space.</param>
/// <param name="Width">Width of the component in meters.</param>
/// <param name="Height">Height of the component in meters.</param>
/// <param name="Depth">Depth of the component in meters.</param>
/// <param name="X">X coordinate of the component position.</param>
/// <param name="Y">Y coordinate of the component position.</param>
/// <param name="Z">Z coordinate of the component position.</param>
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
    /// <param name="component">The domain entity to map from.</param>
    /// <returns>A new DTO populated from the entity.</returns>
    public static LearningComponentDto FromDomain(LearningComponent component) =>
        new(
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
