using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components in a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    private const string EmptyLearningSpaceIdMessage = "Learning space ID cannot be null or empty";

    /// <summary>
    /// Handles the request to list learning components for a given learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>
    /// OK with the component list, BadRequest when the id is invalid,
    /// or NotFound when the learning space does not exist.
    /// </returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return Task.FromResult(LearningComponentHttpResults.BadRequest(EmptyLearningSpaceIdMessage));
        }

        return LearningComponentQueryExecutor.ExecuteAsync(learningComponentService, learningSpaceId);
    }
}
