using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components for a learning space.
/// </summary>
/// <param name="Components">The learning components in the learning space.</param>
public record class GetLearningComponentsResponse(List<LearningComponentDto> Components)
{
    /// <summary>
    /// Creates a response from domain learning components.
    /// </summary>
    /// <param name="components">Domain components to map.</param>
    /// <returns>A response containing mapped DTOs.</returns>
    public static GetLearningComponentsResponse FromEntities(IEnumerable<LearningComponent> components)
    {
        return new GetLearningComponentsResponse(
            components.Select(LearningComponentDto.FromEntity).ToList());
    }
}
