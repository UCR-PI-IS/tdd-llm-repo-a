using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps domain learning component entities to presentation-layer DTOs and responses.
/// </summary>
internal static class LearningComponentMapper
{
    /// <summary>
    /// Converts a list of <see cref="LearningComponent"/> entities into a <see cref="GetLearningComponentsResponse"/>.
    /// </summary>
    /// <param name="components">The domain entities to map.</param>
    /// <returns>A response containing the mapped DTOs.</returns>
    public static GetLearningComponentsResponse ToResponse(List<LearningComponent> components)
    {
        var dtos = components.Select(c => new LearningComponentDto(
            c.ComponentId,
            c.LearningSpaceId,
            c.Width,
            c.Height,
            c.Depth,
            c.X,
            c.Y,
            c.Z,
            c.Orientation)).ToList();

        return new GetLearningComponentsResponse(dtos);
    }
}