using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components of a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the request to list learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// OK with the component list, BadRequest if the ID is invalid, or NotFound if the space does not exist.
    /// </returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return Task.FromResult(LearningComponentsErrorResults.BadRequestEmptyId());
        }

        return LearningComponentsQuery.ExecuteAsync(
            () => learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId));
    }
}
