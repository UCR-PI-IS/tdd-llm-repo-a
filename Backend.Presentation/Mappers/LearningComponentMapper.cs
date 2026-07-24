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
    /// Maps a domain entity to its DTO representation.
    /// </summary>
    public static LearningComponentDto ToDto(LearningComponent component) =>
        new(
            component.ComponentId,
            component.LearningSpaceId,
            component.Width,
            component.Height,
            component.Depth,
            component.X,
            component.Y,
            component.Z,
            component.Orientation);

    /// <summary>
    /// Maps a sequence of domain entities to a list response.
    /// </summary>
    public static GetLearningComponentsResponse ToResponse(IEnumerable<LearningComponent> components) =>
        new(components.Select(ToDto).ToList());
}
