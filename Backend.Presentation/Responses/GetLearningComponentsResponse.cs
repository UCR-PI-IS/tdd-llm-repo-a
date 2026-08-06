using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
public class GetLearningComponentsResponse
{
    /// <summary>
    /// Gets or sets the list of learning components.
    /// </summary>
    public List<LearningComponentDto> Components { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLearningComponentsResponse"/> class.
    /// </summary>
    /// <param name="components">The list of learning components.</param>
    public GetLearningComponentsResponse(List<LearningComponentDto> components)
    {
        Components = components;
    }
}
