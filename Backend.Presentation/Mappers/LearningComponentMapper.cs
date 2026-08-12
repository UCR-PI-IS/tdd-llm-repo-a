using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

public static class LearningComponentMapper
{
    public static GetLearningComponentsResponse ToResponse(List<LearningComponent> components)
    {
        var dtos = components.Select(ToDto).ToList();
        return new GetLearningComponentsResponse(dtos);
    }

    private static LearningComponentDto ToDto(LearningComponent c)
    {
        return new LearningComponentDto(
            c.ComponentId,
            c.LearningSpaceId,
            c.Width,
            c.Height,
            c.Depth,
            c.X,
            c.Y,
            c.Z,
            c.Orientation);
    }
}
