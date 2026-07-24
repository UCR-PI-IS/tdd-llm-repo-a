using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components in a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to list learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// OK with the component list, BadRequest when the id is invalid,
    /// or NotFound when the learning space does not exist.
    /// </returns>
    public static Task<IResult> HandleAsync(
        [FromServices] ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        return LearningComponentListAction.ExecuteAsync(learningComponentService, learningSpaceId);
    }
}
