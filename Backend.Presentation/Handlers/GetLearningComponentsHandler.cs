using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components by learning space ID.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning component data.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A result containing the list of learning components or an error response.</returns>
    public static Task<IResult> HandleAsync(
        [FromServices] ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            return Task.FromResult(LearningComponentResponseBuilder.BadRequest("Learning space ID cannot be null or empty"));
        }

        return LearningComponentResponseBuilder.HandleCoreAsync(learningComponentService, learningSpaceId);
    }
}
