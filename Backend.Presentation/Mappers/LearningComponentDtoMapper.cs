using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;

internal static class LearningComponentDtoMapper
{
    public static GetLearningComponentsResponse ToResponse(IEnumerable<LearningComponent>? components)
    {
        var source = components ?? Enumerable.Empty<LearningComponent>();
        return new GetLearningComponentsResponse
        {
            Components = source.Select(ToDto).ToList()
        };
    }

    private static LearningComponentDto ToDto(LearningComponent component) => new()
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
