using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
/// <param name="Components">The learning components for the requested learning space.</param>
public record class GetLearningComponentsResponse(List<LearningComponentDto> Components);
