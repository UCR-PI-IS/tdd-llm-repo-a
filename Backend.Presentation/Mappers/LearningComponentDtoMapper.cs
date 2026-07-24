using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps learning component entities to DTOs.
/// </summary>
internal static class LearningComponentDtoMapper
{
    /// <summary>
    /// Maps domain components to presentation DTOs.
    /// </summary>
    /// <param name="components">Domain components.</param>
    public static List<LearningComponentDto> ToDtoList(List<LearningComponent> components)
    {
        var dtos = new List<LearningComponentDto>(components.Count);
        foreach (var component in components)
        {
            dtos.Add(new LearningComponentDto(
                component.ComponentId,
                component.LearningSpaceId,
                component.Width,
                component.Height,
                component.Depth,
                component.X,
                component.Y,
                component.Z,
                component.Orientation));
        }

        return dtos;
    }
}
