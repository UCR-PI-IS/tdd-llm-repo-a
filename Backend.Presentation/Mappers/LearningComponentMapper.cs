using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;

/// <summary>
/// Maps domain entities to presentation DTOs.
/// </summary>
public static class LearningComponentMapper
{
    /// <summary>
    /// Maps a LearningComponent domain entity to a LearningComponentDto.
    /// </summary>
    public static LearningComponentDto ToDto(LearningComponent component)
    {
        return new LearningComponentDto
        {
            ComponentId = component.ComponentId,
            LearningSpaceId = component.LearningSpaceId,
            Width = component.Width,
            Height = component.Height,
            Depth = component.Depth,
            X = component.X,
            Y = component.Y,
            Z = component.Z,
            Orientation = component.Orientation
        };
    }
}