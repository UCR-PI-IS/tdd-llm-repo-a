using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response model for GetLearningComponents endpoint
/// </summary>
public class GetLearningComponentsResponse
{
    public List<LearningComponentDto> Components { get; set; } = new List<LearningComponentDto>();
}
