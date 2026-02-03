using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response for getting learning components.
/// </summary>
public class GetLearningComponentsResponse
{
    /// <summary>
    /// List of learning components.
    /// </summary>
    public List<LearningComponentDto> Components { get; set; } = new();
}
