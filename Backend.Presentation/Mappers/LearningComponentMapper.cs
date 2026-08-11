using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps domain <see cref="LearningComponent"/> entities to presentation DTOs.
/// </summary>
internal static class LearningComponentMapper
{
    /// <summary>
    /// Converts a list of <see cref="LearningComponent"/> entities to a list of <see cref="LearningComponentDto"/>.
    /// </summary>
    /// <param name="components">The domain entities to map.</param>
    /// <returns>A list of DTOs representing the given components.</returns>
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
