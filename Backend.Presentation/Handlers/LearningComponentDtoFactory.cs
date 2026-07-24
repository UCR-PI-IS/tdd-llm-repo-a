using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Maps learning component entities to presentation DTOs and responses.
/// </summary>
internal static class LearningComponentDtoFactory
{
    public static GetLearningComponentsResponse ToResponse(IReadOnlyList<LearningComponent> components)
    {
        var dtos = new List<LearningComponentDto>(components.Count);
        foreach (var component in components)
        {
            dtos.Add(MapOne(component));
        }

        return new GetLearningComponentsResponse(dtos);
    }

    private static LearningComponentDto MapOne(LearningComponent component)
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
