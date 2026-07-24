using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps learning component domain entities to presentation DTOs and responses.
/// </summary>
internal static class LearningComponentMapper
{
    /// <summary>
    /// Maps a collection of domain components to a list response.
    /// </summary>
    public static GetLearningComponentsResponse ToResponse(IEnumerable<LearningComponent> components)
    {
        return new GetLearningComponentsResponse(components.Select(ToDto).ToList());
    }

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
}
