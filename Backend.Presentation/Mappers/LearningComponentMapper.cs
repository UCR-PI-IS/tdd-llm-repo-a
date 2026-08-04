using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps LearningComponent entities to DTOs.
/// </summary>
public static class LearningComponentMapper
{
    /// <summary>
    /// Maps a LearningComponent entity to a LearningComponentDto.
    /// </summary>
    /// <param name="component">The learning component entity.</param>
    /// <returns>A LearningComponentDto.</returns>
    public static LearningComponentDto ToDto(LearningComponent component)
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

    /// <summary>
    /// Maps a collection of LearningComponent entities to a list of LearningComponentDto objects.
    /// </summary>
    /// <param name="components">The collection of learning component entities.</param>
    /// <returns>A list of LearningComponentDto objects.</returns>
    public static List<LearningComponentDto> ToDtoList(IEnumerable<LearningComponent> components)
    {
        return components.Select(ToDto).ToList();
    }
}
