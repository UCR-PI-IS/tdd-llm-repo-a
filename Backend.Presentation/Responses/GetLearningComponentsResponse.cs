using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the list of learning components.
/// </summary>
public class GetLearningComponentsResponse
{
    /// <summary>
    /// Gets the list of learning components.
    /// </summary>
    public List<LearningComponent> Components { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLearningComponentsResponse"/> class.
    /// </summary>
    /// <param name="components">The list of learning components.</param>
    public GetLearningComponentsResponse(List<LearningComponent> components)
    {
        Components = components;
    }
}
