using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
public record class GetLearningComponentsResponse(List<LearningComponentDto> Components)
{
    /// <summary>
    /// Creates a response from a collection of learning component entities.
    /// </summary>
    /// <param name="components">The learning component entities.</param>
    /// <returns>A GetLearningComponentsResponse.</returns>
    public static GetLearningComponentsResponse FromEntities(IEnumerable<LearningComponent> components)
    {
        return new GetLearningComponentsResponse(
            components.Select(c => new LearningComponentDto(
                c.ComponentId,
                c.LearningSpaceId,
                c.Width,
                c.Height,
                c.Depth,
                c.X,
                c.Y,
                c.Z,
                c.Orientation)).ToList());
    }
}
