using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps <see cref="LearningComponent"/> domain entities to <see cref="LearningComponentDto"/> instances.
/// </summary>
internal static class LearningComponentMapper
{
    /// <summary>
    /// Converts a list of <see cref="LearningComponent"/> entities to a list of <see cref="LearningComponentDto"/>.
    /// </summary>
    /// <param name="components">The domain entities to map.</param>
    /// <returns>A list of DTOs corresponding to the input entities.</returns>
    public static List<LearningComponentDto> ToDtoList(List<LearningComponent> components)
    {
        return components.Select(c => new LearningComponentDto(
            c.ComponentId,
            c.LearningSpaceId,
            c.Width,
            c.Height,
            c.Depth,
            c.X,
            c.Y,
            c.Z,
            c.Orientation)).ToList();
    }
}
