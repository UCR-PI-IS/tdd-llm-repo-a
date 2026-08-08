using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
public record class GetLearningComponentsResponse(List<LearningComponentDto> Components)
{
    public static GetLearningComponentsResponse FromDomain(List<LearningComponent> components) =>
        new(components.ConvertAll(LearningComponentDto.FromDomain));
}
