using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Contract for handling learning component creation requests.
/// </summary>
public interface ICreateLearningComponentHandler
{
    /// <summary>
    /// Handles the creation of a learning component.
    /// </summary>
    /// <param name="request">The creation request.</param>
    /// <returns>A task containing the handler result.</returns>
    Task<CreateLearningComponentResult> HandleAsync(CreateComponentRequest request);
}
