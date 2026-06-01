using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps domain <see cref="LearningComponent"/> entities to presentation DTOs and responses.
/// </summary>
public static class LearningComponentMapper
{
    /// <summary>
    /// Converts a <see cref="LearningComponent"/> entity to a <see cref="LearningComponentDto"/>.
    /// </summary>
    private static LearningComponentDto ToDto(LearningComponent component)
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
    /// Builds a <see cref="GetLearningComponentsResponse"/> from a list of domain entities.
    /// </summary>
    public static GetLearningComponentsResponse ToResponse(List<LearningComponent> components)
    {
        var dtos = components.Select(ToDto).ToList();
        return new GetLearningComponentsResponse(dtos);
    }
}