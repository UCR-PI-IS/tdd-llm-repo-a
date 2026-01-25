using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response object for getting learning components.
/// </summary>
public class GetLearningComponentsResponse
{
    /// <summary>
    /// Gets or sets the list of learning components.
    /// </summary>
    public List<LearningComponentDto> Components { get; set; } = new();
}
