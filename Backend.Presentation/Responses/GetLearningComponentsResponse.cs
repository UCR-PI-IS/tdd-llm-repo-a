using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
public record class GetLearningComponentsResponse(List<LearningComponent> Components)
{
}
