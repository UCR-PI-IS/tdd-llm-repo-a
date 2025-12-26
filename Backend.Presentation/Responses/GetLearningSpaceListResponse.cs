using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning spaces.
/// </summary>
public record class GetLearningSpaceListResponse(List<LearningSpaceDto> LearningSpaces)
{
}
